using System;
using System.IO;
using System.Reflection;

namespace SiloModelingTaskClient
{
    public static class AssemblyResolver
    {
        private static bool _installed;

        public static void Install()
        {
            if (_installed)
            {
                return;
            }

            AppDomain.CurrentDomain.AssemblyResolve += ResolveFromPluginDirectory;
            _installed = true;
        }

        private static Assembly ResolveFromPluginDirectory(object sender, ResolveEventArgs args)
        {
            string dir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string assemblyName = new AssemblyName(args.Name).Name + ".dll";
            string path = Path.Combine(dir, assemblyName);

            if (File.Exists(path))
            {
                return Assembly.LoadFrom(path);
            }

            return null;
        }
    }
}
