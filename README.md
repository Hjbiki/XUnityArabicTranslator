# XUnity Arabic Translator

**يترجم ألعاب Unity للعربية تلقائياً.**

العربية | [English](#english)

---

## تنبيه مهم

- هذا المشروع **للترجمة فقط** — مو حل لمشاكل الخطوط (Fonts). لو الخط ما يدعم الحروف العربية، النص ما بيظهر صح حتى مع الترجمة.
- الترجمة من **Google Translate** — يعني سيئة صراحة، بس أفضل من ولا شي لو ما تفهم اللغة الأصلية
- يشتغل على **ألعاب Unity فقط**

## التثبيت

### 1. حمّل XUnity.AutoTranslator
حمّل من [هنا](https://github.com/bbepis/XUnity.AutoTranslator/releases):
- **ReiPatcher** — الأسهل ويشتغل مع أغلب الألعاب
- **BepInEx** — لو اللعبة تستخدمه

فك الضغط بمجلد اللعبة وشغّلها مرة.

### 2. حمّل Arabic Translator
حمّل `ArabicGoogleTranslate.dll` من [صفحة التحميل](https://github.com/Hjbiki/XUnityArabicTranslator/releases).

حط الملف بمكانين داخل مجلد اللعبة:
```
{اللعبة}_Data/Managed/ArabicGoogleTranslate.dll
{اللعبة}_Data/Managed/Translators/ArabicGoogleTranslate.dll
```

### 3. عدّل الإعدادات
افتح ملف الإعدادات (يتسوّى تلقائي بعد أول تشغيل) وعدّل:
```ini
[Service]
Endpoint=ArabicGoogleTranslate

[General]
Language=ar
FromLanguage=en
```
غيّر `FromLanguage` حسب لغة اللعبة: `en` إنجليزي، `ja` ياباني، `zh` صيني، `ko` كوري.

### 4. إعدادات منصوح فيها
أضف هالإعدادات عشان تحصل أفضل نتيجة:
```ini
[Behaviour]
MaxCharactersPerTranslation=500
MinDialogueChars=2
ForceUIResizing=True
EnableUIResizing=True
```

| الإعداد | ايش يسوي |
|---|---|
| `MaxCharactersPerTranslation=500` | يترجم نصوص أطول (الافتراضي 200 قليل) |
| `MinDialogueChars=2` | يترجم حتى النصوص القصيرة |
| `ForceUIResizing=True` | يعدّل حجم المربعات عشان النص العربي يسع ولا يتداخل |
| `EnableUIResizing=True` | يفعّل تعديل حجم الواجهة تلقائياً |

### 5. شغّل اللعبة
أول تشغيل يكون أبطأ لأنه يترجم كل النصوص. بعدها تتحفظ الترجمات تلقائياً.

## مشاكل معروفة

| المشكلة | التوضيح |
|---|---|
| بعض النصوص ما تنترجم | عدّل `MaxCharactersPerTranslation=500` و `MinDialogueChars=2`. بعض النصوص XUnity يتجاهلها حسب طريقة عرض اللعبة لها — هذا من XUnity مو من الـ plugin |
| ترتيب الكلام يتلخبط أحياناً | نادر — يصير لما حجم الـ textbox ضيق أو النص طويل. حساب العرض يعتمد على متوسط عرض الحروف |
| النص يتداخل مع عناصر ثانية | فعّل `ForceUIResizing=True` بالإعدادات |

---

<a name="english"></a>

# English

**Automatically translates Unity games to Arabic.**

## Important

- This is a **translation-only** tool — not a font fix. If the game's font doesn't support Arabic characters, text won't display correctly even with this plugin
- Translation quality is from **Google Translate** — it's rough, but better than nothing
- Works with **Unity games only**

## Installation

1. Install [XUnity.AutoTranslator](https://github.com/bbepis/XUnity.AutoTranslator/releases)
2. Download `ArabicGoogleTranslate.dll` from [Releases](https://github.com/Hjbiki/XUnityArabicTranslator/releases)
3. Place the DLL in both `Managed/` and `Managed/Translators/`
4. Set config: `Endpoint=ArabicGoogleTranslate`, `Language=ar`
5. Run the game

## License

MIT

## Credits

- [XUnity.AutoTranslator](https://github.com/bbepis/XUnity.AutoTranslator)
