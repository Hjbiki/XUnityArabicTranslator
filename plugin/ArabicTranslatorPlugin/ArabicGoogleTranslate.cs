using System;
using System.Globalization;
using System.Net;
using System.Text;
using XUnity.AutoTranslator.Plugin.Core.Endpoints;
using XUnity.AutoTranslator.Plugin.Core.Endpoints.Http;
using XUnity.AutoTranslator.Plugin.Core.Web;

namespace ArabicGoogleTranslate
{
    public class ArabicGoogleTranslateEndpoint : HttpEndpoint
    {
        private static readonly string DefaultUrl =
            "https://translate.googleapis.com/translate_a/single?client=gtx&sl={0}&tl={1}&dt=t&q={2}";

        private string _url;
        private string _destLang;
        private bool _autoLineWidth;
        private float _lineWidthScale;
        private int _fixedLineWidth;
        private int _lastSourceLineWidth;

        public override string Id
        {
            get { return "ArabicGoogleTranslate"; }
        }

        public override string FriendlyName
        {
            get { return "Google Translate (Arabic Reshape)"; }
        }

        public override int MaxConcurrency
        {
            get { return 1; }
        }

        public override int MaxTranslationsPerRequest
        {
            get { return 1; }
        }

        public override void Initialize(IInitializationContext context)
        {
            _url = context.GetOrCreateSetting("ArabicGoogle", "ServiceUrl", DefaultUrl);
            _destLang = context.DestinationLanguage;

            _autoLineWidth = context.GetOrCreateSetting("ArabicGoogle", "AutoLineWidth", true);
            _lineWidthScale = context.GetOrCreateSetting("ArabicGoogle", "LineWidthScale", 1.0f);
            _fixedLineWidth = context.GetOrCreateSetting("ArabicGoogle", "FixedLineWidth", 0);
            ArabicReshaper.StripTashkeel = context.GetOrCreateSetting("ArabicGoogle", "StripTashkeel", true);

            context.DisableCertificateChecksFor("translate.googleapis.com");

            if (_destLang != "ar")
                throw new Exception(
                    "ArabicGoogleTranslate is designed for Arabic (ar) output. " +
                    "Set Language=ar in config, or use GoogleTranslate instead.");
        }

        public override void OnCreateRequest(IHttpRequestCreationContext context)
        {
            _lastSourceLineWidth = ArabicReshaper.EstimateArabicCapacity(context.UntranslatedText);

            string address = string.Format(
                _url,
                context.SourceLanguage,
                context.DestinationLanguage,
                Uri.EscapeDataString(context.UntranslatedText));

            var request = new XUnityWebRequest(address);
            request.Headers[HttpRequestHeader.UserAgent] =
                "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36";
            request.Headers[HttpRequestHeader.Accept] = "*/*";

            context.Complete(request);
        }

        private bool _hookAttempted;

        public override void OnExtractTranslation(IHttpTranslationExtractionContext context)
        {
            if (!_hookAttempted)
            {
                _hookAttempted = true;
                try
                {
                    LineReorderer.Setup();
                }
                catch (Exception hookEx)
                {
                    try
                    {
                        System.IO.File.AppendAllText("arabic_debug.log",
                            "[HOOK ERROR] " + hookEx.GetType().Name + ": " + hookEx.Message + Environment.NewLine);
                        if (hookEx.InnerException != null)
                            System.IO.File.AppendAllText("arabic_debug.log",
                                "[HOOK INNER] " + hookEx.InnerException.GetType().Name + ": " + hookEx.InnerException.Message + Environment.NewLine);
                    }
                    catch { }
                }
            }

            string data = context.Response.Data;

            if (string.IsNullOrEmpty(data))
            {
                context.Fail("Empty response from Google Translate");
                return;
            }

            string translation = ParseGoogleResponse(data);

            if (string.IsNullOrEmpty(translation))
            {
                context.Fail("Failed to parse Google Translate response");
                return;
            }

            int maxWidth = 0;
            if (!LineReorderer.IsActive)
            {
                if (_fixedLineWidth > 0)
                    maxWidth = _fixedLineWidth;
                else if (_autoLineWidth && _lastSourceLineWidth > 0)
                    maxWidth = Math.Max(8, (int)(_lastSourceLineWidth * _lineWidthScale));
            }

            translation = ArabicReshaper.Process(translation, maxWidth);

            try
            {
                System.IO.File.AppendAllText("arabic_debug.log",
                    "[" + DateTime.Now.ToString("HH:mm:ss") + "] " +
                    "SrcLen=" + _lastSourceLineWidth +
                    " MaxW=" + maxWidth +
                    " TrLen=" + translation.Length +
                    " HasNL=" + (translation.IndexOf('\n') >= 0) +
                    " Hook=" + LineReorderer.IsActive +
                    " Fixed=" + _fixedLineWidth +
                    " Auto=" + _autoLineWidth +
                    " Scale=" + _lineWidthScale +
                    " Text=[" + (translation.Length > 80 ? translation.Substring(0, 80) + "..." : translation) + "]" +
                    Environment.NewLine);
            }
            catch { }

            context.Complete(translation);
        }

        // ---- Google Translate JSON parser ----

        /// <summary>
        /// Parse the nested-array response from Google Translate (dt=t).
        /// Format: [[["translated","original",...],["translated2","original2",...],...],...]
        /// Extracts and concatenates all translated segments.
        /// </summary>
        private static string ParseGoogleResponse(string data)
        {
            if (string.IsNullOrEmpty(data))
                return null;

            var sb = new StringBuilder();
            int i = 0;
            int depth = 0;
            bool insideTranslations = false;
            bool needFirstString = false;

            while (i < data.Length)
            {
                char c = data[i];

                if (c == '"')
                {
                    if (needFirstString && depth == 3)
                    {
                        i++;
                        string text = ReadJsonString(data, ref i);
                        sb.Append(text);
                        needFirstString = false;
                    }
                    else
                    {
                        i++;
                        SkipJsonString(data, ref i);
                    }
                    continue;
                }

                if (c == '[')
                {
                    depth++;
                    if (depth == 3)
                        needFirstString = true;
                    if (depth == 2 && !insideTranslations)
                        insideTranslations = true;
                    i++;
                    continue;
                }

                if (c == ']')
                {
                    if (insideTranslations && depth == 2)
                        return sb.Length > 0 ? sb.ToString() : null;
                    depth--;
                    i++;
                    continue;
                }

                i++;
            }

            return sb.Length > 0 ? sb.ToString() : null;
        }

        private static string ReadJsonString(string data, ref int pos)
        {
            var sb = new StringBuilder();
            bool escaped = false;

            while (pos < data.Length)
            {
                char c = data[pos];
                pos++;

                if (escaped)
                {
                    switch (c)
                    {
                        case '"': sb.Append('"'); break;
                        case '\\': sb.Append('\\'); break;
                        case '/': sb.Append('/'); break;
                        case 'n': sb.Append('\n'); break;
                        case 'r': sb.Append('\r'); break;
                        case 't': sb.Append('\t'); break;
                        case 'u':
                            if (pos + 4 <= data.Length)
                            {
                                string hex = data.Substring(pos, 4);
                                int code;
                                if (int.TryParse(hex, NumberStyles.HexNumber, null, out code))
                                    sb.Append((char)code);
                                pos += 4;
                            }
                            break;
                        default:
                            sb.Append(c);
                            break;
                    }
                    escaped = false;
                    continue;
                }

                if (c == '\\') { escaped = true; continue; }
                if (c == '"') return sb.ToString();
                sb.Append(c);
            }

            return sb.ToString();
        }

        private static void SkipJsonString(string data, ref int pos)
        {
            bool escaped = false;
            while (pos < data.Length)
            {
                char c = data[pos];
                pos++;
                if (escaped) { escaped = false; continue; }
                if (c == '\\') { escaped = true; continue; }
                if (c == '"') return;
            }
        }
    }
}
