using Microsoft.AspNetCore.Mvc.Filters;
using System.Text.Encodings.Web;
using System.Text.Json;
using XuePing.Application.Common;

namespace XuePing.Api.Middleware
{
    public class GlobalExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<GlobalExceptionMiddleware> _logger;
        public GlobalExceptionMiddleware (RequestDelegate next , ILogger<GlobalExceptionMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }
        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch(Exception ex) 
            {
                _logger.LogError(ex, "请求处理发生未捕获错误:{Path}", context.Request.Path);
                await HandleExceptionAsync(context, ex);
            }
        }
        public static async Task HandleExceptionAsync(HttpContext context , Exception ex)
        {
            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            context.Response.ContentType = "application/json;charset=utf-8 ";
            var result = Result<object>.Fail($"服务器内部错误:{ex.Message}");
            var json = JsonSerializer.Serialize(result,JsonOptions);
            await context.Response.WriteAsync(json);
        }
        public static readonly JsonSerializerOptions JsonOptions = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
        };
    }
}
