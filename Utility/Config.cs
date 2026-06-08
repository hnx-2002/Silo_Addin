namespace SiloModelingTaskClient
{
    /// <summary>
    /// 插件配置
    /// </summary>
    public static class Config
    {
        /// <summary>
        /// 业务接口基础地址
        /// </summary>
        public static string ApiBaseUrl { get; set; } = "http://localhost:6140/PTools_PSilo";

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
