using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DynamicApp.Backend.Middleware
{
    public class ErrorHandlingMiddleware
    {
        private readonly RequestDelegate next;
        public ErrorHandlingMiddleware(RequestDelegate next)
        {
            this.next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await next(context);
            }
            catch(Exception exception)
            {
                if (exception is ApiException)
                {
                    await HandleExceptionAsync(context, exception);
                }
                else
                {
                    throw;
                }
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception ex)
        {
            var apiError = (ApiException)ex;
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)apiError.StatusCode;

            var result = JsonConvert.SerializeObject(new {
                Message = apiError.Message,
                StatusCode = apiError.StatusCode
            });

            await context.Response.WriteAsync(result);
        }
    }
}
