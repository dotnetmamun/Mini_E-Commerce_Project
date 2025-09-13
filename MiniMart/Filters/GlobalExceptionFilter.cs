using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
namespace MiniMart.Filters
{
    

    public class GlobalExceptionFilter : IExceptionFilter
    {
        public void OnException(ExceptionContext context)
        {
            // log error to file
            var logPath = Path.Combine("logs", "errors.txt");
            var msg = $"{DateTime.UtcNow:O} | {context.Exception.GetType().FullName} | {context.Exception.Message} | {context.Exception.StackTrace}";
            Directory.CreateDirectory("logs");
            File.AppendAllText(logPath, msg + Environment.NewLine);

            // optional: in dev you could show details, but in prod redirect to friendly page
            var result = new RedirectToActionResult("Error", "Home", new { area = "" });
            context.Result = result;
            context.ExceptionHandled = true;
        }
    }

}
