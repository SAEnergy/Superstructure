using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Util
{
    public static class AttributeHelper
    {
        public static T GetAttribute<T>(this Type type)
        {
            return (T) type.GetCustomAttributes(typeof(T), true).FirstOrDefault();
        }
    }
}
