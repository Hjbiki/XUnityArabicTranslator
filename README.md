# XUnityArabicTranslator
مترجم مخصص للعربية مع XUnity.AutoTranslator

<a name="lang-ar"></a>العربية | <a name="lang-en"></a>[English](#english-version)

## 📑 المحتويات
- [نبذة](#section-about-ar)
- [المميزات](#section-features-ar)
- [المتطلبات](#section-requirements-ar)
- [التثبيت](#section-installation-ar)
- [الاستخدام](#section-usage-ar)
- [المميزات الخاصة](#section-special-features-ar)
- [الترخيص](#section-license-ar)
- [الشكر والتقدير](#section-credits-ar)

<a name="section-about-ar"></a>
## 📝 نبذة
هذا المشروع مستوحى من [ezTransWeb](https://github.com/HelloKS/ezTransWeb) ولكنه يركز على الترجمة للعربية مع إعادة تشكيل النص.

<a name="section-features-ar"></a>
## ✨ المميزات
- ترجمة متخصصة للغة العربية
- إعادة تشكيل النص العربي بشكل صحيح
- دعم اتجاه النص من اليمين إلى اليسار
- الحفاظ على رموز العملات والأرقام
- دعم جميع اللغات المصدر

<a name="section-requirements-ar"></a>
## 📋 المتطلبات
- Python 3.x
- Flask
- googletrans
- arabic-reshaper
- python-bidi

<a name="section-installation-ar"></a>
## ⚙️ التثبيت
1. قم بتثبيت Python 3.x
2. قم بتثبيت المكتبات المطلوبة:
```bash
pip install -r requirements.txt
```

<a name="section-usage-ar"></a>
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

<a name="section-special-features-ar"></a>
## 💎 المميزات الخاصة
### إعادة تشكيل النص العربي
- يعيد تشكيل النص العربي بشكل صحيح
- يدعم جميع أشكال الحروف العربية
- يحافظ على علامات التشكيل

### الحفاظ على التنسيق
- يحافظ على رموز العملات
- يحافظ على الأرقام
- يحافظ على الرموز الخاصة

<a name="section-license-ar"></a>
## 📄 الترخيص
هذا المشروع مرخص تحت [رخصة MIT](LICENSE).

<a name="section-credits-ar"></a>
## 🙏 الشكر والتقدير
- [ezTransWeb](https://github.com/HelloKS/ezTransWeb) - المشروع الأصلي
- [XUnity.AutoTranslator](https://github.com/bbepis/XUnity.AutoTranslator)
- [arabic-reshaper](https://github.com/mpcabd/python-arabic-reshaper)
- [python-bidi](https://github.com/MeirKriheli/python-bidi)
- [googletrans](https://github.com/ssut/py-googletrans)

---

<a name="english-version"></a>
# XUnityArabicTranslator
Custom Arabic translator for XUnity.AutoTranslator

<a name="lang-en"></a>English | <a name="lang-ar"></a>[العربية](#lang-ar)

## 📑 Table of Contents
- [About](#section-about-en)
- [Features](#section-features-en)
- [Requirements](#section-requirements-en)
- [Installation](#section-installation-en)
- [Usage](#section-usage-en)
- [Special Features](#section-special-features-en)
- [License](#section-license-en)
- [Credits](#section-credits-en)

<a name="section-about-en"></a>
## 📝 About
This project is inspired by [ezTransWeb](https://github.com/HelloKS/ezTransWeb) but focuses on Arabic translation with text reshaping.

<a name="section-features-en"></a>
## ✨ Features
- Specialized Arabic translation
- Proper Arabic text reshaping
- RTL text direction support
- Currency and numbers preservation
- Supports all source languages

<a name="section-requirements-en"></a>
## 📋 Requirements
- Python 3.x
- Flask
- googletrans
- arabic-reshaper
- python-bidi

<a name="section-installation-en"></a>
## ⚙️ Installation
1. Install Python 3.x
2. Install required libraries:
```bash
pip install -r requirements.txt
```

<a name="section-usage-en"></a>
## 🚀 Usage
1. Run the translator:
```bash
python translator.py
```

2. Configure XUnity.AutoTranslator:
```ini
Endpoint=CustomTranslate
[Custom]
Url=http://127.0.0.1:5000/translate
```

<a name="section-special-features-en"></a>
## 💎 Special Features
### Arabic Text Reshaping
- Properly reshapes Arabic text
- Supports all Arabic letter forms
- Preserves diacritics

### Format Preservation
- Preserves currency symbols
- Preserves numbers
- Preserves special characters


<a name="section-license-en"></a>
## 📄 License
This project is licensed under the [MIT License](LICENSE).

<a name="section-credits-en"></a>
## 🙏 Credits
- [ezTransWeb](https://github.com/HelloKS/ezTransWeb) - Original project
- [XUnity.AutoTranslator](https://github.com/bbepis/XUnity.AutoTranslator)
- [arabic-reshaper](https://github.com/mpcabd/python-arabic-reshaper)
- [python-bidi](https://github.com/MeirKriheli/python-bidi)
- [googletrans](https://github.com/ssut/py-googletrans) 
