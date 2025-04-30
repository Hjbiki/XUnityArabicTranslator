# XUnityArabicTranslator
مترجم مخصص للعربية مع XUnity.AutoTranslator

<div style="text-align: right; direction: rtl;">
  <button onclick="switchLanguage('ar')" style="background: none; border: none; cursor: pointer; font-weight: bold;">العربية</button> | 
  <button onclick="switchLanguage('en')" style="background: none; border: none; cursor: pointer;">English</button>
</div>

<div id="ar-content" style="display: block;">
## نبذة
هذا المشروع مستوحى من [ezTransWeb](https://github.com/HelloKS/ezTransWeb) ولكنه يركز على الترجمة للعربية مع إعادة تشكيل النص.

## المميزات
- ترجمة متخصصة للغة العربية
- إعادة تشكيل النص العربي بشكل صحيح
- دعم اتجاه النص من اليمين إلى اليسار
- الحفاظ على رموز العملات والأرقام
- دعم جميع اللغات المصدر

## المتطلبات
- Python 3.x
- Flask
- googletrans
- arabic-reshaper
- python-bidi

## التثبيت
1. قم بتثبيت Python 3.x
2. قم بتثبيت المكتبات المطلوبة:
```bash
pip install -r requirements.txt
```

## الاستخدام
1. قم بتشغيل المترجم:
```bash
python translator.py
```

2. قم بتكوين XUnity.AutoTranslator:
```
Endpoint=CustomTranslate
[Custom]
Url=http://127.0.0.1:5000/translate
```

## المميزات الخاصة
### إعادة تشكيل النص العربي
- يعيد تشكيل النص العربي بشكل صحيح
- يدعم جميع أشكال الحروف العربية
- يحافظ على علامات التشكيل

### الحفاظ على التنسيق
- يحافظ على رموز العملات
- يحافظ على الأرقام
- يحافظ على الرموز الخاصة

## المساهمة
نرحب بمساهماتكم!

## الترخيص
رخصة MIT

## الشكر والتقدير
- [ezTransWeb](https://github.com/HelloKS/ezTransWeb) - المشروع الأصلي
- [XUnity.AutoTranslator](https://github.com/bbepis/XUnity.AutoTranslator)
- [arabic-reshaper](https://github.com/mpcabd/python-arabic-reshaper)
- [python-bidi](https://github.com/MeirKriheli/python-bidi)
- [googletrans](https://github.com/ssut/py-googletrans)
</div>

<div id="en-content" style="display: none;">
## About
This project is inspired by [ezTransWeb](https://github.com/HelloKS/ezTransWeb) but focuses on Arabic translation with text reshaping.

## Features
- Specialized Arabic translation
- Proper Arabic text reshaping
- RTL text direction support
- Currency and numbers preservation
- Supports all source languages

## Requirements
- Python 3.x
- Flask
- googletrans
- arabic-reshaper
- python-bidi

## Installation
1. Install Python 3.x
2. Install required libraries:
```bash
pip install -r requirements.txt
```

## Usage
1. Run the translator:
```bash
python translator.py
```

2. Configure XUnity.AutoTranslator:
```
Endpoint=CustomTranslate
[Custom]
Url=http://127.0.0.1:5000/translate
```

## Special Features
### Arabic Text Reshaping
- Properly reshapes Arabic text
- Supports all Arabic letter forms
- Preserves diacritics

### Format Preservation
- Preserves currency symbols
- Preserves numbers
- Preserves special characters

## Contributing
Contributions are welcome!

## License
MIT License

## Credits
- [ezTransWeb](https://github.com/HelloKS/ezTransWeb) - Original project
- [XUnity.AutoTranslator](https://github.com/bbepis/XUnity.AutoTranslator)
- [arabic-reshaper](https://github.com/mpcabd/python-arabic-reshaper)
- [python-bidi](https://github.com/MeirKriheli/python-bidi)
- [googletrans](https://github.com/ssut/py-googletrans)
</div>

<script>
function switchLanguage(lang) {
  if (lang === 'ar') {
    document.getElementById('ar-content').style.display = 'block';
    document.getElementById('en-content').style.display = 'none';
    document.querySelector('button[onclick="switchLanguage(\'ar\')"]').style.fontWeight = 'bold';
    document.querySelector('button[onclick="switchLanguage(\'en\')"]').style.fontWeight = 'normal';
  } else {
    document.getElementById('ar-content').style.display = 'none';
    document.getElementById('en-content').style.display = 'block';
    document.querySelector('button[onclick="switchLanguage(\'ar\')"]').style.fontWeight = 'normal';
    document.querySelector('button[onclick="switchLanguage(\'en\')"]').style.fontWeight = 'bold';
  }
}
</script> 
