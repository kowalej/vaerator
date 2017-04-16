using Localization.Helpers;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Resources;
using Xamarin.Forms;

namespace Localization.Localize
{
    /// <summary>
    /// Singleton container that manages resource files (.resx / .resw). Allows resource access from anywhere in application using Instance().GetString(....
    /// </summary>
    public sealed class ResourceContainer : IResourceContainer
    {
        private static volatile ResourceContainer instance;
        private static object syncRoot = new Object();

        private Dictionary<string, ResourceManager> resources;
        public CultureInfo Culture { get; private set; }
        private string resourceNamespace = @"Localization.TranslationResources";
        public string ResourceNamespace { get { return resourceNamespace; } set { resourceNamespace = @value; } }

        private ResourceContainer()
        {
            RefreshCulture();
            InitializeResources();
        }

        public static ResourceContainer Instance
        {
            get
            {
                if(instance == null)
                {
                    lock (syncRoot)
                    {
                        if(instance == null)
                            instance = new ResourceContainer();
                    }
                }
                return instance;
            }
        }

        private void InitializeResources()
        {
            resources = new Dictionary<string, ResourceManager>();
            Array translationResourceNames = Enum.GetValues(typeof(Enums.TranslationResources));
            foreach (Enums.TranslationResources resFileNameEnum in translationResourceNames)
            {
                string resFileName = resFileNameEnum.ToDescriptionString();
                string baseName = string.Format("{0}.{1}", resourceNamespace, resFileName);
                Assembly assembly = this.GetType().GetTypeInfo().Assembly;
                resources.Add(resFileName, new ResourceManager(baseName, assembly));
            }
        }

        public bool RefreshCulture()
        {
            CultureInfo newCulture = DependencyService.Get<ILocalize>().GetCurrentCultureInfo();
            if (Culture == null || Culture.Name != newCulture.Name)
            {
                Culture = newCulture;
                DependencyService.Get<ILocalize>().SetLocale(Culture); // Set the Thread for locale-aware methods.
                TranslationResources.ErrorResources.Culture = Culture;
                return true; // Culture changed.
            }
            else return false; // No change.
        }

        public string GetString(string key, Enums.TranslationResources type = default(Enums.TranslationResources))
        {
            if (resources.ContainsKey(type.ToDescriptionString()))
            {
                try
                {
                    return resources[type.ToDescriptionString()].GetString(key, Culture);
                }
                catch (MissingManifestResourceException ex)
                {
                    return string.Empty;
                }
            }
            else return string.Empty;
        }

        public string GetString(string key, string type)
        {
            if (resources.ContainsKey(type))
            {
                try
                {
                    return resources[type].GetString(key, Culture);
                }
                catch (MissingManifestResourceException ex)
                {
                    return string.Empty;
                }
            }
            else return string.Empty;
        }
    }
}
