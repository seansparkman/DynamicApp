using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace DynamicApp.Client
{
    public interface IResponse
    {
        string ErrorMessage { get; set; }
        bool Success { get; set; }
        bool IsTokenExpired { get; set; }
        bool HasInternetConnection { get; set; }
        HttpStatusCode StatusCode { get; set; }
    }
}
