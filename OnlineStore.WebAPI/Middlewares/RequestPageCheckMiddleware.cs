namespace OnlineStore.WebAPI.Middlewares
{
    public class RequestPageCheckMiddleware
    {
        private readonly RequestDelegate _next;

        public RequestPageCheckMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            if (context.Request.RouteValues.TryGetValue("action", out var actionName) && actionName?.ToString() == "GetOrdersFromPage")
            {
                if (context.Request.Query.TryGetValue("pageSize", out var pageSize))
                {
                    if (int.TryParse(pageSize, out int size))
                    {
                        if (size > 20)
                        {
                            context.Response.StatusCode = 400;
                            await context.Response.WriteAsync("pageSize is too large");
                            return;
                        }
                    }
                }
            }

            await _next(context);
        }
    }

    public static class RequestPageCheckMiddlewareExtension
    {
        public static IApplicationBuilder UseRequestPageCheck(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<RequestPageCheckMiddleware>();
        }
    }
}
