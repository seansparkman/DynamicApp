using DynamicApp.Client;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace DynamicApp.Mobile.Services
{
    public class PagesService
    {
        HttpClient _httpClient;
        ApiClient _apiClient;
        public PagesService()
        {
            _httpClient = new HttpClient();
            _apiClient = new ApiClient(_httpClient,
                DependencyService.Get<ISettings>(),
                DependencyService.Get<ILogger>(),
                RefreshToken);
        }


        public async Task<Response> RefreshToken()
        {
            return await Task.FromResult(new Response());
        }
    }
}
