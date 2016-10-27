using System;
using System.Reflection;

namespace Test.Helpers
{
    public static class SingletonHelper
    {
        public static void Clean(Type type)
        {
            var baseType = type.BaseType;

            if (baseType != null)
            {
                var field = baseType.GetField("_singleton", BindingFlags.Static | BindingFlags.NonPublic);

                if (field != null)
                {
                    field.SetValue(null, null);
                }
            }
        }
    }
}
