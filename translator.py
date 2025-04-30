from flask import Flask, request
from googletrans import Translator
import arabic_reshaper
from bidi.algorithm import get_display
import re
import logging

# Set up logging
logging.basicConfig(level=logging.DEBUG)
logger = logging.getLogger(__name__)

app = Flask(__name__)

def decode_text(txt):
    chars = "↔◁◀▷▶♤♠♡♥♧♣⊙◈▣◐◑▒▤▥▨▧▦▩♨☏☎☜☞↕↗↙↖↘♩♬㉿㈜㏇™㏂㏘＂＇∼ˇ˘˝¡˚˙˛¿ː∏￦℉€$¥£₩₽₺₮₱₲₴₳₵₡₢₣₤₥₦₧₨₪₫₭₯₰₱₲₳₴₵₸₹₺₻₼₽₾₿¢$¥£₩₽₺₮₱₲₴₳₵₡₢₣₤₥₦₧₨₪₫₭₯₰₱₲₳₴₵₸₹₺₻₼₽₾₿¢㎕㎖㎗ℓ㎘㎣㎤㎥㎦㎙㎚㎛㎟㎠㎢㏊㎍㏏㎈㎉㏈㎧㎨㎰㎱㎲㎳㎴㎵㎶㎷㎸㎀㎁㎂㎃㎄㎺㎻㎼㎽㎾㎿㎐㎑㎒㎓㎔Ω㏀㏁㎊㎋㎌㏖㏅㎭㎮㎯㏛㎩㎪㎫㎬㏝㏐㏓㏃㏉㏜㏆┒┑┚┙┖┕┎┍┞┟┡┢┦┧┪┭┮┵┶┹┺┽┾╀╁╃╄╅╆╇╈╉╊┱┲ⅰⅱⅲⅳⅴⅵⅶⅷⅸⅹ½⅓⅔¼¾⅛⅜⅝⅞ⁿ₁₂₃₄ŊđĦĲĿŁŒŦħıĳĸŀłœŧŋŉ㉠㉡㉢㉣㉤㉥㉦㉧㉨㉩㉪㉫㉬㉭㉮㉯㉰㉱㉲㉳㉴㉵㉶㉷㉸㉹㉺㉻㈀㈁㈂㈃㈄㈅㈆㈇㈈㈉㈊㈋㈌㈍㈎㈏㈐㈑㈒㈓㈔㈕㈖㈗㈘㈙㈚㈛ⓐⓑⓒⓓⓔⓕⓖⓗⓘⓙⓚⓛⓜⓝⓞⓟⓠⓡⓢⓣⓤⓥⓦⓧⓨⓩ①②③④⑤⑥⑦⑧⑨⑩⑪⑫⑬⑭⑮⒜⒝⒞⒟⒠⒡⒢⒣⒤⒥⒦⒧⒨⒩⒪⒫⒬⒭⒮⒯⒰⒱⒲⒳⒴⒵⑴⑵⑶⑷⑸⑹⑺⑻⑼⑽⑾⑿⒀⒁⒂"
    for c in chars:
        if c in txt:
            txt = txt.replace(c,"\\u" + str(hex(ord(c)))[2:])
    return txt

def encode_text(txt):
    return re.sub(r'(?i)(?<!\\)(?:\\\\)*\\u([0-9a-f]{4})', lambda m: chr(int(m.group(1), 16)), txt)

class FreeTranslator:
    def __init__(self):
        self.translator = Translator()
        
    def translate_text(self, text, from_lang='auto', to_lang='ar'):
        try:
            # Translate using Google Translate
            translation = self.translator.translate(text, src=from_lang, dest=to_lang)
            translated = translation.text
            
            # If target language is Arabic, reshape the text
            if to_lang == 'ar':
                reshaped_text = arabic_reshaper.reshape(translated)
                final_text = get_display(reshaped_text)
                return final_text
                
            return translated
            
        except Exception as e:
            logger.error(f"Translation error: {str(e)}")
            return ""

translator = FreeTranslator()

@app.route("/")
def home():
    return "Free Translator with Arabic Reshaping"

@app.route("/translate")
def webtranslate():
    src_text = request.args.get('text')
    from_lang = request.args.get('from', 'auto')
    to_lang = request.args.get('to', 'ar')
    
    # Log the incoming request
    print(f"Received translation request - From: {from_lang}, To: {to_lang}, Text: {src_text}")
    
    if not src_text:
        print("No text provided in request")
        return ""
    
    try:
        # Decode the input text
        decoded_text = decode_text(src_text)
        print(f"Decoded text: {decoded_text}")
        
        # Translate
        translated = translator.translate_text(decoded_text, from_lang, to_lang)
        print(f"Translated text: {translated}")
        
        # Encode the result
        result = encode_text(translated)
        print(f"Final result: {result}")
        
        return result
    except Exception as e:
        print(f"Error in translation: {str(e)}")
        return ""

if __name__ == '__main__':
    app.run(host="127.0.0.1", port=5000, threaded=True, debug=True) 