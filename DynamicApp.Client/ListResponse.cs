using System.Collections.Generic;
using System.Net;

namespace DynamicApp.Client
{
    public class ListResponse<T> : List<T>, IResponse, IResponseDto where T : IResponseDto
    {
        public string ErrorMessage { get; set; }
        public bool Success { get; set; }
        public bool IsTokenExpired { get; set; }
        public bool HasInternetConnection { get; set; }
        public HttpStatusCode StatusCode { get; set; }

        public void ConvertFromUTC()
        {
            this.ForEach(item => item.ConvertFromUTC());
        }
    }
}
