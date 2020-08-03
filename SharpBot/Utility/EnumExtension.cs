using System;
using SharpBot.Services;

namespace SharpBot.Utility {
    public static class EnumExtension {

        public static T GetAttribute<T>(this Enum value) where T : Attribute {
            var memberInfo = value.GetType().GetMember(value.ToString());
            var attributes = memberInfo[0].GetCustomAttributes(typeof(T), false);
            return attributes.Length > 0 ? (T) attributes[0] : null;
        }
        
        public static string GetUrlAttribute(this Enum value) {
            var attribute = value.GetAttribute<UrlAttr>();
            return attribute == null ? value.ToString() : attribute.Url;
        }
    }
}