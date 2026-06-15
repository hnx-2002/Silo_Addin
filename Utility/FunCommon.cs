using System;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;

namespace SiloModelingTaskClient
{
    /// <summary>
    /// 插件通用工具。
    /// </summary>
    internal static class FunCommon
    {
        /// <summary>
        /// 从主插件读取当前登录Token。
        /// </summary>
        /// <returns>当前登录Token。</returns>
        public static string ReadToken()
        {
            try
            {
                Assembly tabAssembly = AppDomain.CurrentDomain.GetAssemblies()
                    .First(x => x.GetName().Name.StartsWith(Config.RootName));
                Type tokenType = tabAssembly.GetType(Config.RootName + ".Token");
                string token = tokenType.GetMethod("GetCurrentToken").Invoke(null, null).ToString();
                return token;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                return null;
            }
        }

        /// <summary>
        /// 从主插件读取当前平台配置。
        /// </summary>
        /// <returns>当前平台API地址和Web地址。</returns>
        public static (string ApiUrl, string WebUrl) ReadConfig()
        {
            try
            {
                Assembly tabAssembly = AppDomain.CurrentDomain.GetAssemblies()
                    .First(x => x.GetName().Name.StartsWith(Config.RootName));
                Type configType = tabAssembly.GetType(Config.RootName + ".Config");
                string apiUrl = configType.GetMethod("GetCurrentApiUrl").Invoke(null, null).ToString();
                string webUrl = configType.GetMethod("GetCurrentWebUrl").Invoke(null, null).ToString();

                return (apiUrl, webUrl);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                return (null, null);
            }
        }

        /// <summary>
        /// 获取当前插件程序集版本。
        /// </summary>
        /// <returns>当前插件程序集版本。</returns>
        public static string GetCurrentVersion()
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            return assembly.GetName().Version.ToString();
        }
    }
}
