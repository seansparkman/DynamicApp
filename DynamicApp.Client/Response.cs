using System.Net;
using Newtonsoft.Json;

namespace DynamicApp.Client
{
    public class Response : IResponseDto, IResponse
    {
        [JsonIgnore]
        public string ErrorMessage { get; set; }
        [JsonIgnore]
        public bool Success { get; set; }
        [JsonIgnore]
        public bool IsTokenExpired { get; set; }
        [JsonIgnore]
        public bool HasInternetConnection { get; set; }
        [JsonIgnore]
        public HttpStatusCode StatusCode { get; set; }
        public void ConvertFromUTC()
        {
        }
    }
}
