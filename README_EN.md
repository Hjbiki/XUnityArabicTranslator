# XUnityArabicTranslator

A custom Arabic translator with text reshaping for XUnity.AutoTranslator

[English](README_EN.md) | [العربية](README.md)

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