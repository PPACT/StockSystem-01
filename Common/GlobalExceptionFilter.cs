using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using StockSystem.Common;

namespace StockSystem.Common
{
    public class GlobalExceptionFilter : IExceptionFilter
    {
        private readonly ILogger<GlobalExceptionFilter> _logger;

        public GlobalExceptionFilter(ILogger<GlobalExceptionFilter> logger)
        {
            _logger = logger;
        }

        public void OnException(ExceptionContext context)
        {
            var ex = context.Exception;

            // 记录日志
            _logger.LogError(ex, "全局异常：{Message}", ex.Message);

            // 统一返回格式
            var result = ApiResult.Error(ex.Message);

            context.Result = new JsonResult(result);
            context.ExceptionHandled = true;
        }
    }
}