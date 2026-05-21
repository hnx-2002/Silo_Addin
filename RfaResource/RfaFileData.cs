namespace SiloModelingTaskClient
{
    /// <summary>
    /// 导出的族文件数据
    /// </summary>
    public class RfaFileData
    {
        /// <summary>
        /// 族名称
        /// </summary>
        public string FamilyName { get; set; }

        /// <summary>
        /// 族类型名称
        /// </summary>
        public string SymbolName { get; set; }

        /// <summary>
        /// 文件名称
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        /// 文件二进制内容
        /// </summary>
        public byte[] Bytes { get; set; }
    }
}
