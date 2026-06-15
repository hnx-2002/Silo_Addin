namespace SiloModelingTaskClient
{
    /// <summary>
    /// 插件配置
    /// </summary>
    internal class Config
    {
        /// <summary>
        /// 主插件程序集根命名空间。
        /// </summary>
        public static string RootName { get; set; } = "IIESPTabs";

        /// <summary>
        /// 插件编号，用于插件平台鉴权代理。
        /// </summary>
        public static string ToolCode { get; set; } = "PTools_PSilo";

        /// <summary>
        /// 插件平台API地址。
        /// </summary>
        internal static string APIUrl { get; set; }

        /// <summary>
        /// 插件平台Web地址。
        /// </summary>
        internal static string WebUrl { get; set; }

        /// <summary>
        /// 当前插件版本。
        /// </summary>
        public static string Rev { get; set; } = FunCommon.GetCurrentVersion();

        /// <summary>
        /// 新建任务状态码
        /// </summary>
        public static int NewTaskStatus { get; set; } = 10;

        /// <summary>
        /// 建模完成状态码
        /// </summary>
        public static int ModelingDoneStatus { get; set; } = 12;
    }
}
