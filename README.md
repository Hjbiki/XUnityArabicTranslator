# XUnity Arabic Translator

**يترجم ألعاب Unity للعربية تلقائياً.**

العربية | [English](#english)

---

## تنبيه مهم
- هذا المشروع **للترجمة فقط** — مو حل لمشاكل الخطوط.
  لو الخط اللي تستخدمه في اللعبة ما يدعم الحروف العربية بشكل كامل،  
النص **ما راح يظهر صح** حتى بعد الترجمة.

### كيف تضيف خط عربي مدعوم (arabicsdf1):
1. حمل الملف `arabicsdf1` من الريبو.
2. حطه في مسار اللعبة (مكان وجود ال .exe).
3. افتح ملف الـ **config** الخاص بـ XUnityAutoTranslator وحط في سطر:

   ```ini
   [Behaviour]
   FallbackFontTextMeshPro=arabicsdf1
   ```
   **ملاحظة هامة:** هذا الحل ما يشتغل على جميع الألعاب ولا على كل إصدارات محرك Unity. 
   إن شاء الله في المستقبل راح أسوي مشروع منفصل يحتوي على عدة خطوط عربية لإصدارات مختلفة من يونتي.



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

- This project is **translation-only** — it is not a font fix.  
  If the font used in the game does not fully support Arabic characters,  
  the text **will not display correctly** even after translation.

  ### How to add a supported Arabic font (arabicsdf1):

1. Download the file `arabicsdf1` from the repo.
2. Place it in the game directory (next to the `.exe` file).
3. Open the **config** file of XUnityAutoTranslator and add the following line:

   ```ini
   [Behaviour]
   FallbackFontTextMeshPro=arabicsdf1
   ```
   Important Note:
This solution does not work on all games and not on every version of the Unity engine.
Insha'Allah in the future I will create a separate project that includes multiple Arabic fonts for different Unity versions.


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
