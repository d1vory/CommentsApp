using HttpExceptions.Exceptions;
using Newtonsoft.Json;

namespace CommentsApp.Utils;

public class ErrorHandlingMiddleware
{
    readonly RequestDelegate _next;
    
    public ErrorHandlingMiddleware(RequestDelegate next)
    {
        _next = next;
    }
    
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next.Invoke(context);
        }
        catch (HttpException ex)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)ex.StatusCode;
            var body = JsonConvert.SerializeObject(new { message = ex.Message });
            await context.Response.WriteAsync(body);
        }
    }
}