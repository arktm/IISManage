namespace IISManage.Models
{
    /// <summary>
    /// 操作返回消息
    /// </summary>
    public class ResultModel
    {
        public ResultModel()
        {
            
        }
        public ResultModel(bool isSuccess,string message=null,object appendData = null)
        {
            this.IsSuccess = isSuccess;
            this.Message = message;
            this.AppendData = appendData;
        }
        #region prop
        /// <summary>
        /// 一般消息
        /// </summary>
        public string Message { get; set; }
        /// <summary>
        /// 附加信息
        /// </summary>
        public object AppendData { get; set; }
        /// <summary>
        /// 是否成功
        /// </summary>
        public bool IsSuccess { get; set; }
        #endregion
    }
}