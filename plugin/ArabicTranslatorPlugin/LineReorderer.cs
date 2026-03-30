using System;
using System.Reflection;

namespace ArabicGoogleTranslate
{
    internal static class LineReorderer
    {
        private static bool _busy;

        private static PropertyInfo _uguiTextProp;
        private static PropertyInfo _uguiRectTransformProp;
        private static PropertyInfo _uguiPreferredWidthProp;
        private static PropertyInfo _uguiFontProp;
        private static PropertyInfo _uguiFontSizeProp;

        private static PropertyInfo _tmpTextProp;
        private static PropertyInfo _tmpRectTransformProp;
        private static PropertyInfo _tmpPreferredWidthProp;

        public static bool IsActive { get; private set; }

        public static void Setup()
        {
            Assembly harmonyAsm = null;
            foreach (var asm in AppDomain.CurrentDomain.GetAssemblies())
            {
                if (asm.GetName().Name == "0Harmony")
                {
                    harmonyAsm = asm;
                    break;
                }
            }

            if (harmonyAsm == null)
                throw new Exception("0Harmony assembly not found in loaded assemblies");

            Type harmonyType = harmonyAsm.GetType("HarmonyLib.Harmony")
                            ?? harmonyAsm.GetType("Harmony.HarmonyInstance");

            if (harmonyType == null)
                throw new Exception("Harmony class not found (tried HarmonyLib.Harmony and Harmony.HarmonyInstance)");

            Type harmonyMethodType = harmonyAsm.GetType("HarmonyLib.HarmonyMethod")
                                  ?? harmonyAsm.GetType("Harmony.HarmonyMethod");

            if (harmonyMethodType == null)
                throw new Exception("HarmonyMethod class not found");

            object harmonyInstance;
            if (harmonyType.Name == "Harmony")
            {
                harmonyInstance = Activator.CreateInstance(harmonyType,
                    new object[] { "com.arabictranslator.linereorder" });
            }
            else
            {
                var createMethod = harmonyType.GetMethod("Create", BindingFlags.Static | BindingFlags.Public);
                harmonyInstance = createMethod.Invoke(null, new object[] { "com.arabictranslator.linereorder" });
            }

            bool patched = false;
            patched |= TryPatchComponent(harmonyInstance, harmonyType, harmonyMethodType,
                "UnityEngine.UI.Text", nameof(UguiPostfix),
                ref _uguiTextProp, ref _uguiRectTransformProp, ref _uguiPreferredWidthProp);

            patched |= TryPatchComponent(harmonyInstance, harmonyType, harmonyMethodType,
                "TMPro.TMP_Text", nameof(TmpPostfix),
                ref _tmpTextProp, ref _tmpRectTransformProp, ref _tmpPreferredWidthProp);

            IsActive = patched;

            Log("[HOOK SETUP] Patched=" + patched +
                " UGUI=" + (FindType("UnityEngine.UI.Text") != null) +
                " TMP=" + (FindType("TMPro.TMP_Text") != null) +
                " HarmonyVer=" + harmonyAsm.GetName().Version);
        }

        private static bool TryPatchComponent(
            object harmonyInstance, Type harmonyType, Type harmonyMethodType,
            string typeName, string postfixName,
            ref PropertyInfo textProp, ref PropertyInfo rectTransformProp, ref PropertyInfo preferredWidthProp)
        {
            try
            {
                var type = FindType(typeName);
                if (type == null) return false;

                textProp = type.GetProperty("text");
                rectTransformProp = type.GetProperty("rectTransform");
                preferredWidthProp = type.GetProperty("preferredWidth");

                var setter = textProp != null ? textProp.GetSetMethod() : null;
                if (setter == null || preferredWidthProp == null) return false;

                var postfixMethodInfo = typeof(LineReorderer).GetMethod(postfixName,
                    BindingFlags.Static | BindingFlags.NonPublic);
                var hmPostfix = Activator.CreateInstance(harmonyMethodType, new object[] { postfixMethodInfo });

                var patchMethod = harmonyType.GetMethod("Patch", new Type[]
                {
                    typeof(MethodBase),
                    harmonyMethodType,
                    harmonyMethodType,
                    harmonyMethodType
                });

                if (patchMethod != null)
                {
                    patchMethod.Invoke(harmonyInstance,
                        new object[] { setter, null, hmPostfix, null });
                }
                else
                {
                    patchMethod = harmonyType.GetMethod("Patch", new Type[]
                    {
                        typeof(MethodBase),
                        harmonyMethodType,
                        harmonyMethodType,
                        harmonyMethodType,
                        harmonyMethodType
                    });
                    if (patchMethod != null)
                        patchMethod.Invoke(harmonyInstance,
                            new object[] { setter, null, hmPostfix, null, null });
                    else
                        return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                Log("[PATCH FAIL " + typeName + "] " + ex.GetType().Name + ": " + ex.Message);
                return false;
            }
        }

        private static Type FindType(string fullName)
        {
            foreach (var asm in AppDomain.CurrentDomain.GetAssemblies())
            {
                var type = asm.GetType(fullName);
                if (type != null) return type;
            }
            return null;
        }

        static void UguiPostfix(object __instance)
        {
            ProcessUgui(__instance);
        }

        static void TmpPostfix(object __instance)
        {
            ProcessGeneric(__instance, _tmpTextProp, _tmpRectTransformProp, _tmpPreferredWidthProp);
        }

        private static void ProcessUgui(object instance)
        {
            if (_busy) return;

            try
            {
                string text = (string)_uguiTextProp.GetValue(instance, null);
                if (string.IsNullOrEmpty(text)) return;
                if (!HasArabicPresentationForms(text)) return;
                if (text.IndexOf('\n') >= 0) return;

                float rectWidth = GetRectWidth(instance, _uguiRectTransformProp);
                if (rectWidth <= 0) return;

                float preferredWidth = (float)_uguiPreferredWidthProp.GetValue(instance, null);
                if (preferredWidth <= rectWidth) return;

                string reordered = WrapByPixelWidth(text, rectWidth, preferredWidth, instance);
                if (reordered == null || reordered == text) return;

                _busy = true;
                _uguiTextProp.SetValue(instance, reordered, null);
            }
            catch { }
            finally
            {
                _busy = false;
            }
        }

        private static void ProcessGeneric(
            object instance,
            PropertyInfo textProp,
            PropertyInfo rectTransformProp,
            PropertyInfo preferredWidthProp)
        {
            if (_busy) return;

            try
            {
                string text = (string)textProp.GetValue(instance, null);
                if (string.IsNullOrEmpty(text)) return;
                if (!HasArabicPresentationForms(text)) return;
                if (text.IndexOf('\n') >= 0) return;

                float rectWidth = GetRectWidth(instance, rectTransformProp);
                if (rectWidth <= 0) return;

                float preferredWidth = (float)preferredWidthProp.GetValue(instance, null);
                if (preferredWidth <= rectWidth) return;

                string reordered = WrapByPixelWidth(text, rectWidth, preferredWidth, null);
                if (reordered == null || reordered == text) return;

                _busy = true;
                textProp.SetValue(instance, reordered, null);
            }
            catch { }
            finally
            {
                _busy = false;
            }
        }

        /// <summary>
        /// Wrap text using pixel-width measurement when possible,
        /// falling back to character-count ratio.
        /// </summary>
        private static string WrapByPixelWidth(string text, float rectWidth, float preferredWidth, object uguiInstance)
        {
            float avgCharWidth = preferredWidth / text.Length;
            string[] words = text.Split(' ');

            var lines = new System.Collections.Generic.List<string>();
            var currentLine = new System.Text.StringBuilder();
            float currentWidth = 0;

            for (int w = 0; w < words.Length; w++)
            {
                string word = words[w];
                if (word.Length == 0) continue;

                float wordWidth = word.Length * avgCharWidth;
                float spaceWidth = currentLine.Length > 0 ? avgCharWidth : 0;

                if (currentLine.Length > 0 && currentWidth + spaceWidth + wordWidth > rectWidth)
                {
                    lines.Add(currentLine.ToString());
                    currentLine.Length = 0;
                    currentWidth = 0;
                    spaceWidth = 0;
                }

                if (currentLine.Length > 0)
                {
                    currentLine.Append(' ');
                    currentWidth += avgCharWidth;
                }
                currentLine.Append(word);
                currentWidth += wordWidth;
            }

            if (currentLine.Length > 0)
                lines.Add(currentLine.ToString());

            if (lines.Count <= 1)
                return null;

            lines.Reverse();
            return string.Join("\n", lines.ToArray());
        }

        private static float GetRectWidth(object component, PropertyInfo rectTransformProp)
        {
            try
            {
                var rectTransform = rectTransformProp.GetValue(component, null);
                if (rectTransform == null) return 0;
                var rectProp = rectTransform.GetType().GetProperty("rect");
                if (rectProp == null) return 0;
                object rect = rectProp.GetValue(rectTransform, null);
                var widthProp = rect.GetType().GetProperty("width");
                return (float)widthProp.GetValue(rect, null);
            }
            catch { return 0; }
        }

        private static bool HasArabicPresentationForms(string text)
        {
            for (int i = 0; i < text.Length; i++)
            {
                char c = text[i];
                if ((c >= '\uFB50' && c <= '\uFDFF') || (c >= '\uFE70' && c <= '\uFEFF'))
                    return true;
            }
            return false;
        }

        private static void Log(string msg)
        {
            try { System.IO.File.AppendAllText("arabic_debug.log", msg + Environment.NewLine); }
            catch { }
        }
    }
}
