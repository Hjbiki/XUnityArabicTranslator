# XUnityArabicTranslator
مترجم مخصص للعربية مع XUnity.AutoTranslator

<a name="lang-en"></a>[English](README_EN.md) | <a name="lang-ar"></a>العربية

## 📑 المحتويات
- [نبذة](#section-about)
- [المميزات](#section-features)
- [المتطلبات](#section-requirements)
- [التثبيت](#section-installation)
- [الاستخدام](#section-usage)
- [المميزات الخاصة](#section-special-features)
- [المساهمة](#section-contributing)
- [الترخيص](#section-license)
- [الشكر والتقدير](#section-credits)

<a name="section-about"></a>
## 📝 نبذة
هذا المشروع مستوحى من [ezTransWeb](https://github.com/HelloKS/ezTransWeb) ولكنه يركز على الترجمة للعربية مع إعادة تشكيل النص.

<a name="section-features"></a>
## ✨ المميزات
- ترجمة متخصصة للغة العربية
- إعادة تشكيل النص العربي بشكل صحيح
- دعم اتجاه النص من اليمين إلى اليسار
- الحفاظ على رموز العملات والأرقام
- دعم جميع اللغات المصدر

<a name="section-requirements"></a>
## 📋 المتطلبات
- Python 3.x
- Flask
- googletrans
- arabic-reshaper
- python-bidi

<a name="section-installation"></a>
## ⚙️ التثبيت
1. قم بتثبيت Python 3.x
2. قم بتثبيت المكتبات المطلوبة:
```bash
pip install -r requirements.txt
```

<a name="section-usage"></a>
## 🚀 الاستخدام
1. قم بتشغيل المترجم:
```bash
python translator.py
```

2. قم بتكوين XUnity.AutoTranslator:
```ini
Endpoint=CustomTranslate
[Custom]
Url=http://127.0.0.1:5000/translate
```

<a name="section-special-features"></a>
## 💎 المميزات الخاصة
### إعادة تشكيل النص العربي
- يعيد تشكيل النص العربي بشكل صحيح
- يدعم جميع أشكال الحروف العربية
- يحافظ على علامات التشكيل

### الحفاظ على التنسيق
- يحافظ على رموز العملات
- يحافظ على الأرقام
- يحافظ على الرموز الخاصة

<a name="section-contributing"></a>
## 🤝 المساهمة
نرحب بمساهماتكم! يمكنكم المساهمة من خلال:
- فتح قضايا جديدة
- تقديم طلبات سحب
- تحسين التوثيق
- إضافة ميزات جديدة

<a name="section-license"></a>
## 📄 الترخيص
هذا المشروع مرخص تحت [رخصة MIT](LICENSE).

<a name="section-credits"></a>
## 🙏 الشكر والتقدير
- [ezTransWeb](https://github.com/HelloKS/ezTransWeb) - المشروع الأصلي
- [XUnity.AutoTranslator](https://github.com/bbepis/XUnity.AutoTranslator)
- [arabic-reshaper](https://github.com/mpcabd/python-arabic-reshaper)
- [python-bidi](https://github.com/MeirKriheli/python-bidi)
- [googletrans](https://github.com/ssut/py-googletrans) 
