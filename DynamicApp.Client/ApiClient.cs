using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace DynamicApp.Client
{
    public class ApiClient
    {
        private const string ACCESS_TOKEN = "access_token";
        private const string ACCESS_TOKEN_HEADER = "x-iina-token";
        private const string IDENTIFIER = "x-iina-identifier";
        private const string MIME_TYPE = "application/json";

        HttpClient _httpClient;
        ISettings _settings;
        ILogger _logger;
        Func<Task<Response>> _refreshToken;
        public ApiClient(HttpClient httpClient, ISettings settings, ILogger logger, Func<Task<Response>> refreshToken)
        {
            _httpClient = httpClient;
            _settings = settings;
            _logger = logger;
            _refreshToken = refreshToken;
        }

        public async Task SetAccessToken(string token)
        {
            if (!string.IsNullOrWhiteSpace(token))
            {
                var claims = await ParseToken(token);
                await _settings.SetClaims(claims);
                await _settings.AddOrUpdateValue(ACCESS_TOKEN, token);
            }
        }

        public async Task<string> GetAccessToken()
        {
            return await _settings.GetValue(ACCESS_TOKEN);
        }

        public async Task ClearAccessToken()
        {
            _httpClient.DefaultRequestHeaders.Remove("Authorization");
            await _settings.Remove(ACCESS_TOKEN);
        }

        public async Task<TResponse> MakeRequest<TResponse>(HttpMethod httpMethod, string path, bool requiresAuth = true, bool refresh = true, bool authFailClearToken = true, string temporaryToken = null, CancellationToken token = default(CancellationToken))
            where TResponse : IResponse, IResponseDto, new()
        {
            return await MakeRequest<TResponse, EmptyRequest>(httpMethod, path, null, requiresAuth, refresh, authFailClearToken, temporaryToken, token);
        }

        public async Task<TResponse> MakeRequest<TResponse, TRequest>(HttpMethod httpMethod, string path, TRequest request, bool requiresAuth = true, bool refresh = true, bool authFailClearToken = true, string temporaryToken = null, CancellationToken token = default(CancellationToken))
            where TResponse : IResponse, IResponseDto, new()
            where TRequest : IRequestDto
        {
            HttpRequestMessage requestMessage = null;
            HttpResponseMessage response = null;

            try
            {
                var jsonContent = string.Empty;
                UrlBuilder urlBuilder = new UrlBuilder(_settings.GetBaseUrl(), path);

                if (httpMethod == HttpMethod.Get && request != null)
                {
                    urlBuilder.Query = GetQueryStringParameters(request);
                }

                requestMessage = new HttpRequestMessage(httpMethod, urlBuilder.ToString());
                await _logger.Trace($"Request: {requestMessage.Method} {requestMessage.RequestUri.ToString()}");

                if (requiresAuth)
                {
                    var authToken = await CheckExpiration(refresh);

                    requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", temporaryToken ?? authToken);
                }

                requestMessage.Headers.TryAddWithoutValidation(IDENTIFIER, await _settings.GetIdentifier());

                if (request != null)
                {
                    request.ConvertToUtc();
                    // TODO: Should eventually convert this to using a stream instead of a string
                    // This would allow for a performance boost and allow for larger data sets
                    jsonContent = JsonConvert.SerializeObject(request);
                    await _logger.Trace($"Request body:\r\n {JsonConvert.SerializeObject(request, Formatting.Indented)}");
                    requestMessage.Content = new StringContent(jsonContent, Encoding.UTF8, MIME_TYPE);
                }

                await _logger.Info("Sending request to API");
                response = await _httpClient.SendAsync(requestMessage, token);
                return await HandleResponse<TResponse>(response, jsonContent, authFailClearToken);
            }
            catch(Exception exception)
            {
                var results = new TResponse();

                await _logger.Error($"Request failed: {exception.Message}\r\n{exception.StackTrace}", exception, requestMessage, response);

                if (exception.Message == "Error: NameResolutionFailure")
                {
                    results.Success = false;
                    results.ErrorMessage = "You do not seem to have an internet connection. Please verify your connectivity.";
                    results.HasInternetConnection = false;
                }
                else
                {
                    results.Success = false;
                    results.ErrorMessage = exception.Message;
                }
                return results;
            }
        }

        public async Task<string> CheckExpiration(bool refresh)
        {
            var authToken = await GetAccessToken();
            var claims = await ParseToken(authToken);

            if (claims != null && refresh && claims.IsPastHalfLife)
            {
                _refreshToken?.Invoke();

                return await GetAccessToken();
            }

            return authToken;
        }

        protected async Task<T> HandleResponse<T>(HttpResponseMessage response, string jsonContent, bool authFailClearToken)
            where T : IResponse, IResponseDto, new()
        {
            T result = default(T);

            await _logger.Trace($"Request Headers: {response.RequestMessage.Headers}");

            if (response != null && response.IsSuccessStatusCode)
            {
                await _logger.Info("API Request Success");
                if (response.Content != null && response.StatusCode == HttpStatusCode.OK)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    await _logger.Trace($"API Response:\r\n${json}");

                    if (!string.IsNullOrWhiteSpace(json))
                    {
                        result = JsonConvert.DeserializeObject<T>(json);
                        result.ConvertFromUTC();
                    }
                }
                if (result == null)
                {
                    result = new T();
                }

                await _logger.Trace($"Response Headers: {response.Headers}");
                // Update the access token if the API returns the proper header
                IEnumerable<string> accessToken;
                if (response.Headers.TryGetValues(ACCESS_TOKEN_HEADER, out accessToken))
                {
                    if (accessToken.Count() > 0)
                    {
                        await SetAccessToken(accessToken.First());
                    }                    
                }

                result.Success = true;
            }
            else if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                await _logger.Warn($"API Request Auth Token Failed: {response.StatusCode} {response.ReasonPhrase}");
                result = new T { Success = false, ErrorMessage = response.ReasonPhrase, IsTokenExpired = true };
                if (authFailClearToken)
                {
                    await ClearAccessToken();
                }

            }
            else
            {
                var message = $"API Request Failed: {response.StatusCode} {response.ReasonPhrase}";
                await _logger.Error(message, new Exception(message), response.RequestMessage, response);
                var error = await ReadError(response);
                result = new T { Success = false, ErrorMessage = error };
            }

            result.StatusCode = response.StatusCode;

            return result;
        }

        private async Task<string> ReadError(HttpResponseMessage message)
        {
            var body = await message.Content.ReadAsStringAsync();

            if (body.StartsWith("{"))
            {
                var jObject = JObject.Parse(body);
                if (jObject.ContainsKey("text"))
                {
                    return (string)jObject["text"];
                }
                if (jObject.ContainsKey("message"))
                {
                    return (string)jObject["message"];
                }
            }

            return message.ReasonPhrase;
        }

        private Dictionary<string, string> GetQueryStringParameters<T>(T request)
            where T : IRequestDto
        {
            if (request != null)
            {
                return request.GetType()
                    .GetProperties(BindingFlags.Instance | BindingFlags.Public)
                    .ToDictionary(prop => prop.Name, prop => prop.GetValue(request, null)?.ToString());
            }
            return null;
        }

        private async Task<Claims> ParseToken(string accessToken)
        {
            if (!string.IsNullOrWhiteSpace(accessToken) && accessToken.Contains('.'))
            {
                var encryptedToken = accessToken.Split('.')[1];

                var jsonClaims = Base64Decode(encryptedToken);

                await _logger.Trace($"Claims: {jsonClaims}");

                return JsonConvert.DeserializeObject<Claims>(jsonClaims);
            }

            return null;
        }

        private string Base64Decode(string token)
        {
            _logger.Trace($"Auth Token: {token}");
            var base64 = token.Replace('-', '+')
                              .Replace('_', '/');

            switch (base64.Length % 4)
            {
                case 0:
                    break;
                case 2:
                    base64 += "==";
                    break;
                case 3:
                    base64 += "=";
                    break;
                default:
                    throw new ArgumentException("Invalid base 64 string", "base64");
            }

            var bytes = Convert.FromBase64String(base64);

            return Encoding.UTF8.GetString(bytes, 0, bytes.Length);
        }
    }
}
