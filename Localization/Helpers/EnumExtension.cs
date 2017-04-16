using Localization.Enums;
using System;
using System.Linq;
using System.Reflection;

namespace Localization.Helpers
{
    public static class EnumExtension
    {
        public static T GetAttributeOfType<T>(this Enum enumValue) where T : Attribute
        {
            var typeInfo = enumValue.GetType().GetTypeInfo();
            var value = typeInfo.DeclaredMembers.First(x => x.Name == enumValue.ToString());
            return value.GetCustomAttribute<T>();
        }

        public static string ToDescriptionString(this Enum value)
        {
            var attribute = GetAttributeOfType<EnumDescription>(value);
            return attribute != null ? attribute.Description : string.Empty;
        }
    }
}
