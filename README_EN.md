# XUnityArabicTranslator
Custom Arabic translator for XUnity.AutoTranslator

English  | [العربية](README.md)

## 📑 Table of Contents
- [About](#section-about)
- [Features](#section-features)
- [Requirements](#section-requirements)
- [Installation](#section-installation)
- [Usage](#section-usage)
- [Special Features](#section-special-features)
- [Contributing](#section-contributing)
- [License](#section-license)
- [Credits](#section-credits)

<a name="section-about"></a>
## 📝 About
This project is inspired by [ezTransWeb](https://github.com/HelloKS/ezTransWeb) but focuses on Arabic translation with text reshaping.

<a name="section-features"></a>
## ✨ Features
- Specialized Arabic translation
- Proper Arabic text reshaping
- RTL text direction support
- Currency and numbers preservation
- Supports all source languages

<a name="section-requirements"></a>
## 📋 Requirements
- Python 3.x
- Flask
- googletrans
- arabic-reshaper
- python-bidi

<a name="section-installation"></a>
## ⚙️ Installation
1. Install Python 3.x
2. Install required libraries:
```bash
pip install -r requirements.txt
```

<a name="section-usage"></a>
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

<a name="section-special-features"></a>
## 💎 Special Features
### Arabic Text Reshaping
- Properly reshapes Arabic text
- Supports all Arabic letter forms
- Preserves diacritics

### Format Preservation
- Preserves currency symbols
- Preserves numbers
- Preserves special characters

<a name="section-contributing"></a>
## 🤝 Contributing
Contributions are welcome! You can contribute by:
- Opening new issues
- Submitting pull requests
- Improving documentation
- Adding new features

<a name="section-license"></a>
## 📄 License
This project is licensed under the [MIT License](LICENSE).

<a name="section-credits"></a>
## 🙏 Credits
- [ezTransWeb](https://github.com/HelloKS/ezTransWeb) - Original project
- [XUnity.AutoTranslator](https://github.com/bbepis/XUnity.AutoTranslator)
- [arabic-reshaper](https://github.com/mpcabd/python-arabic-reshaper)
- [python-bidi](https://github.com/MeirKriheli/python-bidi)
- [googletrans](https://github.com/ssut/py-googletrans) 
