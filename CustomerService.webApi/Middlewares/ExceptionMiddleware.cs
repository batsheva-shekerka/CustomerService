using CustomerService.webApi.Exceptions;
using Microsoft.AspNetCore.Http;
using System;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;


namespace CustomerService.webApi.Middlewares
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;

        // מקבלים את "התחנה הבאה" בשרשרת
        public ExceptionMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        // הפונקציה שמופעלת על כל בקשה שמגיעה לשרת
        public async Task InvokeAsync(HttpContext httpContext)
        {
            try
            {
                // מעבירים את הבקשה הלאה לקונטרולרים
                await _next(httpContext);
            }
            catch (Exception ex)
            {
                // אם פתאום קרתה שגיאה באחד הקונטרולרים, אנחנו תופסים אותה כאן!
                await HandleExceptionAsync(httpContext, ex);
            }
        }

        // איך לטפל בשגיאה
        private static Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            var code = HttpStatusCode.InternalServerError; // ברירת מחדל 500
            var message = "שגיאת שרת פנימית.";

            // אם השגיאה היא מסוג ה-Exception הייעודי שלנו
            if (exception is DuplicateException)
            {
                code = HttpStatusCode.Conflict; // משנים ל-409
                message = exception.Message;
            }

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)code;

            var result = JsonSerializer.Serialize(new { error = message });
            return context.Response.WriteAsync(result);
        }
    }
}
