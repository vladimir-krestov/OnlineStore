using System.Diagnostics;

namespace OnlineStore.WebAPI.Middlewares
{
    public class RequestIdMiddleware
    {
        private readonly RequestDelegate _requestDelegate;

        public RequestIdMiddleware(RequestDelegate requestDelegate)
        {
            _requestDelegate = requestDelegate;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            Guid requestId = Guid.NewGuid();

            context.Items["X-Request-ID"] = requestId.ToString();

            Console.WriteLine($"Request Id: {requestId}");

            await _requestDelegate(context);

            Debug.WriteLine("RequestIdMiddleware after next");
        }
    }

    public static class RequestIdMiddlewareExtension
    {
        public static IApplicationBuilder UseRequestId(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<RequestIdMiddleware>();
        }
    }
}
