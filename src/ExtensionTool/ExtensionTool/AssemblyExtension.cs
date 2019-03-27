using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ExtensionTool
{
    public static class AssemblyExtension
    {
        public static IEnumerable<TResult> GetInstancesFromAssembly<TResult>(this Assembly ass)
        {
            return ass.GetTypes()
                    .Where(x => typeof(TResult).IsAssignableFrom(x) && x.IsNormalClass())
                    .Select(Activator.CreateInstance)
                    .Cast<TResult>();
        }

        public static bool IsNormalClass(this Type type) {
            return type.IsClass && !type.IsAbstract;
        }
    }
}
