using System;
using System.Net;

namespace DynamicApp.Backend.Middleware
{
    public class ApiException
        : Exception
    {
        public ApiException(HttpStatusCode statusCode)
            : this (statusCode, statusCode.ToString())
        {
            
        }
        public ApiException(HttpStatusCode statusCode, string message)
            : base(message)
        {
            StatusCode = statusCode;
        }
        public HttpStatusCode StatusCode { get; set; }
    }
}
