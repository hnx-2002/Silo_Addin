using System;
using System.IO;
using System.Reflection;

namespace SiloModelingTaskClient
{
    /// <summary>
    /// 插件程序集依赖解析器
    /// </summary>
    public static class AssemblyResolver
    {
        private static bool _installed;

        /// <summary>
        /// 安装程序集解析事件
        /// </summary>
        public static void Install()
        {
            if (_installed)
            {
                return;
            }

            AppDomain.CurrentDomain.AssemblyResolve += ResolveFromPluginDirectory;
            _installed = true;
        }

        /// <summary>
        /// 从插件目录解析程序集依赖
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args">程序集解析参数</param>
        /// <returns>解析到的程序集</returns>
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
