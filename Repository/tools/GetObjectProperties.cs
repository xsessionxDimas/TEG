using System.Reflection;

namespace Repository.tools
{
    public static class GetObjectProperties
    {
        public static PropertyInfo[] GetProperties(object target)
        {
            return (target.GetType()).GetProperties();
        }
    }
}
