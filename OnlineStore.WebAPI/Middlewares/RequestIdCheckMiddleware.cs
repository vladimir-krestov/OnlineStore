namespace OnlineStore.WebAPI.Middlewares
{
    public class RequestIdCheckMiddleware
    {
        private readonly RequestDelegate _requestDelegate;

        public RequestIdCheckMiddleware(RequestDelegate requestDelegate)
        {
            _requestDelegate = requestDelegate;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            const string headerKey = "X-Request-ID";

            context.Response.OnStarting(() =>
            {
                if (!context.Items.TryGetValue(headerKey, out var value))
                {
                    throw new InvalidOperationException($"Headers don't contain header: {headerKey}");
                }

                if (!context.Response.Headers.ContainsKey(headerKey))
                {
                    context.Response.Headers.Add("X-Request-ID", value.ToString());
                }

                return Task.CompletedTask;
            });

            Console.WriteLine($"Check header {headerKey}");

            await _requestDelegate(context);
        }
    }

    public static class RequestIdCheckMiddlewareExtension
    {
        public static IApplicationBuilder UseRequestIdCheck(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<RequestIdCheckMiddleware>();
        }
    }
}
