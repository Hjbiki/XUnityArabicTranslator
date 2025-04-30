# XUnityArabicCustomTranslator
مترجم مخصص للعربية مع XUnity.AutoTranslator

A custom Arabic translator with text reshaping for XUnity.AutoTranslator

## نبذة | About
هذا المشروع مستوحى من [ezTransWeb](https://github.com/HelloKS/ezTransWeb) ولكنه يركز على الترجمة للعربية مع إعادة تشكيل النص. | This project is inspired by [ezTransWeb](https://github.com/HelloKS/ezTransWeb) but focuses on Arabic translation with text reshaping.

## المميزات | Features
- ترجمة متخصصة للغة العربية | Specialized Arabic translation
- إعادة تشكيل النص العربي بشكل صحيح | Proper Arabic text reshaping
- دعم اتجاه النص من اليمين إلى اليسار | RTL text direction support
- الحفاظ على رموز العملات والأرقام | Currency and numbers preservation
- دعم جميع اللغات المصدر | Supports all source languages

## المتطلبات | Requirements
- Python 3.x
- Flask
- googletrans
- arabic-reshaper
- python-bidi

## التثبيت | Installation
1. قم بتثبيت Python 3.x | Install Python 3.x
2. قم بتثبيت المكتبات المطلوبة | Install required libraries:
```bash
pip install -r requirements.txt
```

## الاستخدام | Usage
1. قم بتشغيل المترجم | Run the translator:
```bash
python translator.py
```

2. قم بتكوين XUnity.AutoTranslator | Configure XUnity.AutoTranslator:
```
Endpoint=CustomTranslate
[Custom]
Url=http://127.0.0.1:5000/translate
```

## المميزات الخاصة | Special Features
### إعادة تشكيل النص العربي | Arabic Text Reshaping
- يعيد تشكيل النص العربي بشكل صحيح | Properly reshapes Arabic text
- يدعم جميع أشكال الحروف العربية | Supports all Arabic letter forms
- يحافظ على علامات التشكيل | Preserves diacritics

### الحفاظ على التنسيق | Format Preservation
- يحافظ على رموز العملات | Preserves currency symbols
- يحافظ على الأرقام | Preserves numbers
- يحافظ على الرموز الخاصة | Preserves special characters

## المساهمة | Contributing
نرحب بمساهماتكم! | Contributions are welcome!

## الترخيص | License
MIT License

## الشكر والتقدير | Credits
- [ezTransWeb](https://github.com/HelloKS/ezTransWeb) - المشروع الأصلي | Original project
- [XUnity.AutoTranslator](https://github.com/bbepis/XUnity.AutoTranslator)
- [arabic-reshaper](https://github.com/mpcabd/python-arabic-reshaper)
- [python-bidi](https://github.com/MeirKriheli/python-bidi)
- [googletrans](https://github.com/ssut/py-googletrans) 
