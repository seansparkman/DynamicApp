using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace DynamicApp.Client
{
    public interface ILogger
    {
        Task Error(string message, Exception ex, HttpRequestMessage requestMessage, HttpResponseMessage responseMessage);
        Task Error(string message);
        Task Warn(string message);
        Task Info(string message);
        Task Trace(string message);
    }
}
