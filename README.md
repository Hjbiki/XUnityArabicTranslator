# XUnityArabicTranslator
مترجم مخصص للعربية مع XUnity.AutoTranslator

[English](README_EN.md) | [العربية](README.md)

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
