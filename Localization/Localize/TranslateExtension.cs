using Localization.Helpers;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Localization.Localize
{
    // You exclude the 'Extension' suffix when using in Xaml markup
    [ContentProperty("Text")]
    public class TranslateExtension : IMarkupExtension
    {
        ResourceContainer translationResources;

        public TranslateExtension()
        {
            translationResources = ResourceContainer.Instance;
        }

        public string Text { get; set; }

        public object ProvideValue(IServiceProvider serviceProvider)
        {
            string key = Text;
            string type = default(Enums.TranslationResources).ToDescriptionString();
            string translation = string.Empty;

            if (Text == null)
                return "";
            else if (Text.Contains("."))
            {
                string[] parts = Text.Split(new char[] { '.' }, 2); // Split into 2 parts, "type.key" format.
                type = parts[0];
                key = parts[1];
                translation = translationResources.GetString(key, type);
            }
            else
            {
                translation = translationResources.GetString(key); // Try key with default type since no specific type referenced.
            }

            if (string.IsNullOrEmpty(translation))
            {
                #if DEBUG
                    throw new ArgumentException(String.Format("Key: '{0}' was not found in Resources: '{1}' for Culture: '{2}'. Make sure the resource file name was added to LocalizationEnums.", key, type, translationResources.Culture), "Text");
                #else
                    translation = Text; // HACK: returns the Text, which GETS DISPLAYED TO THE USER.
                #endif
            }

            return translation;
        }
    }
}
