using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Core.Util
{
    public static class TypeLocator
    {
        public static IEnumerable<Type> FindTypes(string searchPattern, Type supportedInterface)
        {
            //todo: this should do the search in an appdomain, then destroy it, so files are not locked.
            List<Type> found = new List<Type>();

            var files = Directory.GetFiles(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), searchPattern, SearchOption.TopDirectoryOnly);

            foreach (string file in files)
            {
                var assm = Assembly.LoadFile(file);
                var types = assm.GetTypes().Where(t => supportedInterface.IsAssignableFrom(t) && t.ContainsGenericParameters == false);
                found.AddRange(types);
            }

            return found;
        }
    }
}
