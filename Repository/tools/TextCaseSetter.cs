using System.Reflection;
using CustomTypes.Attributes;
using CustomTypes.Base;

namespace Repository.tools
{
    public static class TextCaseSetter
    {
        public static string Set(BaseEntityObject objData, PropertyInfo property)
        {
            string result;
            var markedUCWord    = (UCWordsFieldAttribute)System.Attribute.GetCustomAttribute(property, typeof(UCWordsFieldAttribute));
            var markedLowerCase = (LowerCaseFieldAttribute)System.Attribute.GetCustomAttribute(property, typeof(LowerCaseFieldAttribute));
            result = (markedUCWord != null) ? StringManipulation.UppercaseFirst(property.GetValue(objData, null).ToString()) : property.GetValue(objData, null).ToString();
            result = (markedLowerCase != null) ? result.ToLower() : result;
            return result;
        }
    }
}
