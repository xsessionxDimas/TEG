using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using CustomTypes.Attributes;
using CustomTypes.Base;
using Repository.Enums;

namespace Repository.tools
{
    public static class RoutinesParameterSetter
    {
        public static void Set(ref SqlCommand command, BaseEntityObject objData, CRUDType crudType)
        {
            var properties = GetObjectProperties.GetProperties(objData);
            foreach (var propertyInfo in properties)
            {
                var insertAttribute = (ByPassInsertParamAttribute) System.Attribute.GetCustomAttribute(propertyInfo, typeof(ByPassInsertParamAttribute));
                var updateAttribute = (ByPassUpdateParamAttribute) System.Attribute.GetCustomAttribute(propertyInfo, typeof(ByPassUpdateParamAttribute));
                var activeAttribute = (ActiveFieldAttribute)System.Attribute.GetCustomAttribute(propertyInfo, typeof(ActiveFieldAttribute));
                var nullAttribute   = (NullFieldAttribute)System.Attribute.GetCustomAttribute(propertyInfo, typeof(NullFieldAttribute)); 
                switch (crudType)
                {
                    case CRUDType.Insert:
                        if(insertAttribute != null) continue;
                        break;
                    case CRUDType.Update :
                        if (updateAttribute != null) continue;
                        break;
                    case CRUDType.Delete :
                        break;
                }
                if(activeAttribute != null || nullAttribute != null)
                {
                    command.Parameters.AddWithValue("@" + propertyInfo.Name, propertyInfo.GetValue(objData, null));
                }
                else
                {
                    command.Parameters.AddWithValue("@" + propertyInfo.Name, TextCaseSetter.Set(objData, propertyInfo));
                }
            }
        }

        public static void Set(ref SqlCommand command, List<Dictionary<string, object>> keyValuePairParam)
        {
            foreach (var dictionary in keyValuePairParam.SelectMany(param => param))
            {
                command.Parameters.AddWithValue("@" + dictionary.Key, dictionary.Value);
            }
        }
    }
}
