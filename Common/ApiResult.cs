namespace StockSystem.Common
{
    // 统一接口返回格式
    public class ApiResult
    {
        public int Code { get; set; }
        public string Msg { get; set; }
        public object? Data { get; set; }

        public static ApiResult Success(object? data = null, string msg = "成功")
        {
            return new ApiResult { Code = 200, Msg = msg, Data = data };
        }

        public static ApiResult Error(string msg = "失败", int code = 500)
        {
            return new ApiResult { Code = code, Msg = msg, Data = null };
        }
    }
}