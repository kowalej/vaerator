﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Localization.TranslationResources {
    using System;
    using System.Reflection;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "4.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    public class BeverageResources {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal BeverageResources() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        public static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("Localization.TranslationResources.BeverageResources", typeof(BeverageResources).GetTypeInfo().Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   Overrides the current thread's CurrentUICulture property for all
        ///   resource lookups using this strongly typed resource class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        public static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Start Aerating.
        /// </summary>
        public static string AerateStartButtonLabel {
            get {
                return ResourceManager.GetString("AerateStartButtonLabel", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Stop Aerating.
        /// </summary>
        public static string AerateStopButtonLabel {
            get {
                return ResourceManager.GetString("AerateStopButtonLabel", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to {0}s.
        /// </summary>
        public static string DurationFormat {
            get {
                return ResourceManager.GetString("DurationFormat", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to {0}s {1}.
        /// </summary>
        public static string DurationText {
            get {
                return ResourceManager.GetString("DurationText", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to (recommended).
        /// </summary>
        public static string Recommended {
            get {
                return ResourceManager.GetString("Recommended", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Aerate Red Wine and Rosé.
        /// </summary>
        public static string RedWinePageTitle {
            get {
                return ResourceManager.GetString("RedWinePageTitle", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Aerate Whiskey and Spirits.
        /// </summary>
        public static string WhiskeyPageTitle {
            get {
                return ResourceManager.GetString("WhiskeyPageTitle", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Aerate White Wine.
        /// </summary>
        public static string WhiteWinePageTitle {
            get {
                return ResourceManager.GetString("WhiteWinePageTitle", resourceCulture);
            }
        }
    }
}
