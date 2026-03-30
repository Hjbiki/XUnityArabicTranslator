"""
Translator v8 — Optimized for XUnity.AutoTranslator
- Multi-translator fallback (Google → MyMemory → rotate)
- FastAPI + uvicorn (async)
- Persistent disk cache
- Pre-warm cache from XUnity translation files
- Placeholder approach for reshaped Arabic names
- RTL Unicode marks
- Live translation log with stats
"""

from fastapi import FastAPI
from fastapi.responses import PlainTextResponse
from starlette.requests import Request
import uvicorn
from deep_translator import GoogleTranslator, MyMemoryTranslator
import arabic_reshaper
from bidi.algorithm import get_display
import re
import json
import os
import asyncio
from concurrent.futures import ThreadPoolExecutor
from collections import OrderedDict
from contextlib import asynccontextmanager
import time
import sys

# --- Config ---
CACHE_FILE = "translation_cache.json"
MAX_CACHE_SIZE = 16384
MAX_TEXT_LENGTH = 5000
WORKER_THREADS = 6
AUTO_SAVE_INTERVAL = 120
RTL_MARK = "\u200F"
FORCE_RTL = False


# --- Live Stats ---

class TranslationStats:
    def __init__(self):
        self.total_requests = 0
        self.cache_hits = 0
        self.api_calls = {}  # per-translator counts
        self.errors = {}     # per-translator errors
        self.start_time = time.time()
        self.is_translating = False

    def log(self, msg):
        elapsed = time.time() - self.start_time
        mins = int(elapsed // 60)
        secs = int(elapsed % 60)
        print(f"[{mins:02d}:{secs:02d}] {msg}", flush=True)

    def add_call(self, name):
        self.api_calls[name] = self.api_calls.get(name, 0) + 1

    def add_error(self, name):
        self.errors[name] = self.errors.get(name, 0) + 1

    def status_line(self):
        hit_rate = (self.cache_hits / self.total_requests * 100) if self.total_requests > 0 else 0
        calls_str = ", ".join(f"{k}: {v}" for k, v in self.api_calls.items()) or "0"
        errs_str = ", ".join(f"{k}: {v}" for k, v in self.errors.items()) or "0"
        return (
            f"Requests: {self.total_requests} | "
            f"Cache: {self.cache_hits} ({hit_rate:.0f}%) | "
            f"API [{calls_str}] | "
            f"Errors [{errs_str}]"
        )

stats = TranslationStats()


# --- Text encoding/decoding ---

SPECIAL_CHARS = "↔◁◀▷▶♤♠♡♥♧♣⊙€$¥£₩₽₺₮₱₲₴₳₵₡₢₣₤₦₨₪₫₭₯₰₱₲₳₴₵₸₹₺₻₼₽₾₿¢"
ENCODE_TABLE = str.maketrans({c: "\\u" + format(ord(c), '04x') for c in SPECIAL_CHARS})
UNICODE_ESCAPE_RE = re.compile(r'(?i)(?<!\\)(?:\\\\)*\\u([0-9a-f]{4})')


def decode_text(txt):
    return txt.translate(ENCODE_TABLE)


def encode_text(txt):
    return UNICODE_ESCAPE_RE.sub(lambda m: chr(int(m.group(1), 16)), txt)


# --- Arabic reshaping ---

def is_reshaped_arabic_char(c):
    return '\uFB50' <= c <= '\uFDFF' or '\uFE70' <= c <= '\uFEFF'


def has_reshaped_arabic(text):
    return any(is_reshaped_arabic_char(c) for c in text)


RESHAPED_ARABIC_RE = re.compile(r'[\uFB50-\uFDFF\uFE70-\uFEFF]+')


def replace_reshaped_with_placeholders(text):
    placeholders = {}
    counter = 0

    def replacer(match):
        nonlocal counter
        key = f"XARB{counter}"
        placeholders[key] = match.group(0)
        counter += 1
        return key

    modified = RESHAPED_ARABIC_RE.sub(replacer, text)
    return modified, placeholders


def restore_placeholders(text, placeholders):
    for key, original in placeholders.items():
        text = text.replace(key, original)
    return text


def reshape_arabic(text):
    reshaped = arabic_reshaper.reshape(text)
    display = get_display(reshaped)
    if FORCE_RTL:
        display = RTL_MARK + display
    return display


# --- Multi-Translator Engine ---

# MyMemory uses locale codes (ar-SA) instead of simple codes (ar)
MYMEMORY_LANG_MAP = {
    'ar': 'ar-SA', 'en': 'en-GB', 'ja': 'ja-JP', 'ko': 'ko-KR',
    'zh': 'zh-CN', 'zh-CN': 'zh-CN', 'zh-TW': 'zh-TW',
    'fr': 'fr-FR', 'de': 'de-DE', 'es': 'es-ES', 'it': 'it-IT',
    'pt': 'pt-PT', 'ru': 'ru-RU', 'tr': 'tr-TR', 'vi': 'vi-VN',
    'th': 'th-TH', 'id': 'id-ID', 'ms': 'ms-MY', 'nl': 'nl-NL',
    'pl': 'pl-PL', 'uk': 'uk-UA', 'cs': 'cs-CZ', 'ro': 'ro-RO',
    'hu': 'hu-HU', 'sv': 'sv-SE', 'da': 'da-DK', 'fi': 'fi-FI',
    'el': 'el-GR', 'he': 'he-IL', 'hi': 'hi-IN', 'bn': 'bn-IN',
}


class TranslatorEngine:
    """
    Multi-translator with automatic fallback.
    
    Google is primary (best quality).
    MyMemory is fallback (different rate limits).
    If one fails, tries the next. Tracks cooldowns to avoid
    hammering a rate-limited service.
    """

    def __init__(self):
        self.cooldowns = {}  # translator_name -> resume_time
        self.COOLDOWN_SECONDS = 30  # wait 30s before retrying a failed translator

    def _is_cooled_down(self, name):
        if name in self.cooldowns:
            if time.time() < self.cooldowns[name]:
                return False
            else:
                del self.cooldowns[name]
        return True

    def _set_cooldown(self, name):
        self.cooldowns[name] = time.time() + self.COOLDOWN_SECONDS

    def _try_google(self, text, from_lang, to_lang):
        """Google Translate — best quality, may rate limit"""
        name = "Google"
        if not self._is_cooled_down(name):
            return None, name

        try:
            result = GoogleTranslator(source=from_lang, target=to_lang).translate(text)
            if result:
                stats.add_call(name)
                return result, name
        except Exception as e:
            err_msg = str(e).lower()
            # If rate limited, set cooldown
            if 'too many' in err_msg or '429' in err_msg or 'rate' in err_msg:
                stats.log(f"⚠ {name}: Rate limited — cooling down {self.COOLDOWN_SECONDS}s")
                self._set_cooldown(name)
            stats.add_error(name)

        return None, name

    def _try_mymemory(self, text, from_lang, to_lang):
        """MyMemory — free backup, different rate limits"""
        name = "MyMemory"
        if not self._is_cooled_down(name):
            return None, name

        try:
            # Convert language codes
            mm_from = MYMEMORY_LANG_MAP.get(from_lang, from_lang)
            mm_to = MYMEMORY_LANG_MAP.get(to_lang, to_lang)

            # MyMemory doesn't support 'auto', default to English
            if mm_from == 'en':
                mm_from = 'en-GB'

            result = MyMemoryTranslator(source=mm_from, target=mm_to).translate(text)
            if result:
                stats.add_call(name)
                return result, name
        except Exception as e:
            err_msg = str(e).lower()
            if 'too many' in err_msg or '429' in err_msg or 'rate' in err_msg or 'limit' in err_msg:
                stats.log(f"⚠ {name}: Rate limited — cooling down {self.COOLDOWN_SECONDS}s")
                self._set_cooldown(name)
            stats.add_error(name)

        return None, name

    def translate(self, text, from_lang, to_lang):
        """
        Try translators in a loop until one succeeds.
        NEVER gives up — keeps rotating between translators.
        Each attempt waits a bit longer to avoid hammering.
        """
        attempt = 0
        max_wait = 10  # cap wait time at 10 seconds between attempts

        while True:
            attempt += 1

            # Try Google
            result, name = self._try_google(text, from_lang, to_lang)
            if result:
                if attempt > 1:
                    stats.log(f"✓ Succeeded on attempt {attempt} via {name}")
                return result, name

            # Try MyMemory
            result, name = self._try_mymemory(text, from_lang, to_lang)
            if result:
                if attempt > 1:
                    stats.log(f"✓ Succeeded on attempt {attempt} via {name}")
                return result, name

            # Both failed — wait and retry
            wait = min(attempt * 2, max_wait)  # 2s, 4s, 6s, 8s, 10s, 10s...
            stats.log(f"🔄 Attempt {attempt} failed — retrying in {wait}s...")
            time.sleep(wait)


engine = TranslatorEngine()


# --- Persistent LRU Cache ---

class PersistentCache:
    def __init__(self, filepath, maxsize=16384):
        self._cache = OrderedDict()
        self._maxsize = maxsize
        self._filepath = filepath
        self._dirty = False
        self._load()

    def _load(self):
        if os.path.exists(self._filepath):
            try:
                with open(self._filepath, 'r', encoding='utf-8') as f:
                    data = json.load(f)
                items = list(data.items())
                if len(items) > self._maxsize:
                    items = items[-self._maxsize:]
                self._cache = OrderedDict(items)
                stats.log(f"Cache: loaded {len(self._cache)} translations from disk")
            except Exception as e:
                stats.log(f"Cache: could not load — {e}")

    def save(self):
        if not self._dirty:
            return
        try:
            with open(self._filepath, 'w', encoding='utf-8') as f:
                json.dump(dict(self._cache), f, ensure_ascii=False)
            self._dirty = False
            stats.log(f"Cache: saved {len(self._cache)} translations to disk")
        except Exception as e:
            stats.log(f"Cache: could not save — {e}")

    def get(self, key):
        if key in self._cache:
            self._cache.move_to_end(key)
            return self._cache[key]
        return None

    def set(self, key, value):
        if key in self._cache:
            self._cache.move_to_end(key)
        else:
            if len(self._cache) >= self._maxsize:
                self._cache.popitem(last=False)
        self._cache[key] = value
        self._dirty = True

    def __len__(self):
        return len(self._cache)


# --- Pre-warm ---

def prewarm_from_xunity(cache, file_path, to_lang='ar'):
    if not os.path.exists(file_path):
        return 0

    count = 0
    try:
        with open(file_path, 'r', encoding='utf-8-sig') as f:
            for line in f:
                line = line.strip()
                if not line or line.startswith('//') or line.startswith('#'):
                    continue
                if '=' in line:
                    original, _, translated = line.partition('=')
                    original = original.strip()
                    translated = translated.strip()
                    if original and translated:
                        if to_lang == 'ar' and not has_reshaped_arabic(translated):
                            translated = reshape_arabic(translated)
                        cache_key = f"{original}:auto:{to_lang}"
                        cache.set(cache_key, translated)
                        count += 1
    except Exception as e:
        stats.log(f"Pre-warm error: {e}")

    stats.log(f"Pre-warm: loaded {count} translations from XUnity file")
    return count


# --- Main Translator ---

class FreeTranslator:
    def __init__(self):
        self.executor = ThreadPoolExecutor(max_workers=WORKER_THREADS)
        self.cache = PersistentCache(CACHE_FILE, maxsize=MAX_CACHE_SIZE)

    def _translate_single_line(self, text, from_lang, to_lang):
        """Translate one line with placeholder protection + fallback"""
        if not text.strip():
            return text

        placeholders = {}
        if has_reshaped_arabic(text):
            text, placeholders = replace_reshaped_with_placeholders(text)

        # engine.translate loops until it succeeds — never returns None
        translated, used_translator = engine.translate(text, from_lang, to_lang)

        if to_lang == 'ar':
            translated = reshape_arabic(translated)

        if placeholders:
            translated = restore_placeholders(translated, placeholders)

        return translated

    def _translate_and_reshape(self, text, from_lang, to_lang):
        r"""
        Split by literal \n (the two characters \ and n, NOT newline char).
        Games use literal \n for line breaks in their text engine.
        
        "笑ってないですよ？\nむしろ安心しました"
          → part 1: "笑ってないですよ？"  → "أنت لا تبتسم، أليس كذلك؟"
          → part 2: "むしろ安心しました"   → "في الواقع، شعرت بالارتياح"
          → rejoin: "أنت لا تبتسم، أليس كذلك؟\nفي الواقع، شعرت بالارتياح"
        """
        # Literal \n (backslash + n) — NOT actual newline
        LITERAL_NEWLINE = '\\n'

        if LITERAL_NEWLINE in text:
            parts = text.split(LITERAL_NEWLINE)
            translated_parts = []
            for part in parts:
                if part.strip():
                    translated_parts.append(
                        self._translate_single_line(part, from_lang, to_lang)
                    )
                else:
                    translated_parts.append(part)
            return LITERAL_NEWLINE.join(translated_parts)

        # Also handle actual newline character (just in case)
        if '\n' in text:
            lines = text.split('\n')
            translated_lines = []
            for line in lines:
                if line.strip():
                    translated_lines.append(
                        self._translate_single_line(line, from_lang, to_lang)
                    )
                else:
                    translated_lines.append(line)
            return '\n'.join(translated_lines)

        return self._translate_single_line(text, from_lang, to_lang)

    async def translate_text(self, text, from_lang='auto', to_lang='ar'):
        if not text or not text.strip():
            return text

        if len(text) > MAX_TEXT_LENGTH:
            text = text[:MAX_TEXT_LENGTH]

        stats.total_requests += 1

        cache_key = f"{text}:{from_lang}:{to_lang}"
        cached = self.cache.get(cache_key)
        if cached is not None:
            stats.cache_hits += 1
            return cached

        preview = text[:60].replace('\n', '\\n') + ("..." if len(text) > 60 else "")
        stats.log(f"▶ Translating: {preview}")
        stats.is_translating = True

        # No timeout — engine.translate retries until success
        # Other requests still work because this runs in a thread pool
        loop = asyncio.get_event_loop()
        result = await loop.run_in_executor(
            self.executor,
            self._translate_and_reshape, text, from_lang, to_lang
        )

        stats.is_translating = False
        self.cache.set(cache_key, result)

        result_preview = result[:60].replace('\n', '\\n') + ("..." if len(result) > 60 else "")
        stats.log(f"✓ Result: {result_preview}")

        return result


translator = FreeTranslator()


# --- App lifecycle ---

async def periodic_cache_save():
    while True:
        await asyncio.sleep(AUTO_SAVE_INTERVAL)
        translator.cache.save()


async def periodic_status():
    last_count = 0
    while True:
        await asyncio.sleep(30)
        if stats.total_requests > last_count:
            stats.log(f"📊 {stats.status_line()} | Cache size: {len(translator.cache)}")
            last_count = stats.total_requests


@asynccontextmanager
async def lifespan(app):
    xunity_paths = [
        "_AutoGeneratedTranslations.txt",
        "Translation/_AutoGeneratedTranslations.txt",
        "AutoTranslator/Translation/_AutoGeneratedTranslations.txt",
    ]
    for path in xunity_paths:
        if os.path.exists(path):
            prewarm_from_xunity(translator.cache, path)
            break

    asyncio.create_task(periodic_cache_save())
    asyncio.create_task(periodic_status())

    print("=" * 55)
    print("  XUnity Arabic Translator v8")
    print(f"  http://127.0.0.1:5000/translate")
    print(f"  Cache: {len(translator.cache)} entries loaded")
    print(f"  RTL marks: {'ON' if FORCE_RTL else 'OFF'}")
    print(f"  Translators: Google → MyMemory (fallback)")
    print("=" * 55)
    stats.log("Server ready — waiting for translations...")

    yield

    translator.cache.save()
    translator.executor.shutdown(wait=False)
    stats.log("Server stopped. Cache saved.")


app = FastAPI(lifespan=lifespan)


# --- Routes ---

@app.get("/", response_class=PlainTextResponse)
async def home():
    return "XUnity Arabic Translator v8"


@app.get("/translate", response_class=PlainTextResponse)
async def webtranslate(request: Request):
    params = dict(request.query_params)
    src_text = params.get('text', '')
    from_lang = params.get('from', 'auto')
    to_lang = params.get('to', 'ar')

    if not src_text:
        return ""
    try:
        decoded = decode_text(src_text)
        translated = await translator.translate_text(decoded, from_lang, to_lang)
        return encode_text(translated)
    except Exception:
        return ""


@app.get("/stats", response_class=PlainTextResponse)
async def get_stats():
    cooldown_info = ""
    for name, resume_time in engine.cooldowns.items():
        remaining = max(0, int(resume_time - time.time()))
        cooldown_info += f"\n  {name}: cooling down ({remaining}s remaining)"

    return (
        f"XUnity Arabic Translator v8\n"
        f"{'=' * 40}\n"
        f"{stats.status_line()}\n"
        f"Cache size: {len(translator.cache)}\n"
        f"RTL marks: {'ON' if FORCE_RTL else 'OFF'}\n"
        f"Currently translating: {'Yes' if stats.is_translating else 'No'}\n"
        f"Translator cooldowns: {cooldown_info if cooldown_info else 'None'}\n"
    )


@app.get("/cache/save", response_class=PlainTextResponse)
async def force_save():
    translator.cache.save()
    return f"Saved {len(translator.cache)} entries"


if __name__ == '__main__':
    if sys.platform == 'win32':
        sys.stdout.reconfigure(encoding='utf-8', errors='replace')

    uvicorn.run(app, host="127.0.0.1", port=5000, log_level="warning")