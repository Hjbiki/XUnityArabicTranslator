from flask import Flask, request
from deep_translator import GoogleTranslator
import arabic_reshaper
from bidi.algorithm import get_display
import re

app = Flask(__name__)

# Define special characters to preserve (minimal set)
SPECIAL_CHARS = "↔◁◀▷▶♤♠♡♥♧♣⊙€$¥£₩₽₺₮₱₲₴₳₵₡₢₣₤₦₨₪₫₭₯₰₱₲₳₴₵₸₹₺₻₼₽₾₿¢"
CHAR_DICT = {c: "\\u" + str(hex(ord(c)))[2:] for c in SPECIAL_CHARS}

def decode_text(txt):
    """Replace special characters with Unicode escape sequences."""
    result = ""
    for c in txt:
        result += CHAR_DICT.get(c, c)
    return result

def encode_text(txt):
    """Convert Unicode escape sequences back to characters."""
    return re.sub(r'(?i)(?<!\\)(?:\\\\)*\\u([0-9a-f]{4})', lambda m: chr(int(m.group(1), 16)), txt)

def translate_text(text, from_lang='auto', to_lang='ar'):
    """Translate text as fast as possible."""
    # Quick return for empty text
    if not text.strip():
        return text

    # Translate directly
    translated = GoogleTranslator(source=from_lang, target=to_lang).translate(text)

    # Reshape for Arabic if needed
    if to_lang == 'ar':
        reshaped_text = arabic_reshaper.reshape(translated)
        return get_display(reshaped_text)

    return translated

@app.route("/")
def home():
    return "Free Translator with Arabic Reshaping"

@app.route("/translate")
def webtranslate():
    src_text = request.args.get('text', '')
    from_lang = request.args.get('from', 'auto')
    to_lang = request.args.get('to', 'ar')

    if not src_text:
        return ""

    # Process directly: decode, translate, encode
    decoded_text = decode_text(src_text)
    translated = translate_text(decoded_text, from_lang, to_lang)
    return encode_text(translated)

if __name__ == '__main__':
    app.run(host="127.0.0.1", port=5000, threaded=True, debug=False)