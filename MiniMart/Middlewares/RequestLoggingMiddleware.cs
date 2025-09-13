
using System.Text;

namespace MiniMart.Middlewares
{
    public class RequestLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly string _logFile = Path.Combine("logs", "requests.txt");

        public RequestLoggingMiddleware(RequestDelegate next)
        {
            _next = next;
            // ensure logs folder exists
            Directory.CreateDirectory("logs");
        }

        public async Task Invoke(HttpContext context)
        {
            var entry = $"{DateTime.UtcNow:O} | {context.Request.Method} {context.Request.Path}{context.Request.QueryString} | RemoteIP: {context.Connection.RemoteIpAddress}";
            // async append
            await File.AppendAllTextAsync(_logFile, entry + Environment.NewLine, Encoding.UTF8);

            await _next(context);
        }
    }

}
