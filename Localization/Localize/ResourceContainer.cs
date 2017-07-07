using Localization.Helpers;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
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
        private string resourceNamespace = @"Localization.TranslationResources";
        public string ResourceNamespace { get { return resourceNamespace; } set { resourceNamespace = @value; } }

        private CultureInfo culture;
        public CultureInfo Culture
        {
            get { return culture; }
            set
            {
                culture = value;
                CultureInfo.DefaultThreadCurrentCulture = value;
                CultureInfo.DefaultThreadCurrentUICulture = value;
            }
        }

        private ResourceContainer()
        {
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
            Assembly assembly = this.GetType().GetTypeInfo().Assembly;
            List<string> resourceNames = assembly.GetManifestResourceNames().ToList(); // Gets all resource files.

            foreach (var resName in resourceNames)
            {
                string baseName = resName.Remove(resName.LastIndexOf('.')); // Strip out .resources
                string key = baseName.Replace(ResourceNamespace + ".", ""); // Strip out namespace for key
                resources.Add(key, new ResourceManager(baseName, assembly));
            }
        }

        public bool RefreshCulture()
        {
            CultureInfo newCulture = DependencyService.Get<ILocalize>().GetCurrentCultureInfo();
            if (Culture == null || Culture.Name != newCulture.Name)
            {
                Culture = newCulture;
                return true; // Culture changed.
            }
            else return false; // No change.
        }

        public string GetString(string key, Enums.TranslationResourcesFiles type = default(Enums.TranslationResourcesFiles))
        {
            return GetString(type.ToDescriptionString());
        }

        public string GetString(string key, string type)
        {
            if (resources.ContainsKey(type))
            {
                try
                {
                    return resources[type].GetString(key, Culture);
                }
                catch (MissingManifestResourceException)
                {
                    return string.Empty;
                }
            }
            else return string.Empty;
        }

        public List<string> GetAllResourceKeys(Enums.TranslationResourcesFiles type)
        {
            return GetAllResourceKeys(type.ToDescriptionString());
        }

        public List<string> GetAllResourceKeys(string type)
        {
            List<string> keys = new List<string>();

            if (resources.ContainsKey(type))
            {
                foreach (var prop in Type.GetType(string.Format("{0}.{1}", resourceNamespace, type)).GetRuntimeProperties())
                {
                    if(prop.Name != "Culture" && prop.Name != "ResourceManager")
                        keys.Add(prop.Name);
                }
            }

            return keys;
        }
    }
}
