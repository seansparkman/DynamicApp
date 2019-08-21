using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DynamicApp.Client
{
    public class UrlBuilder
    {
        public UrlBuilder(string baseUrl, string path = null, string queryString = null, Dictionary<string, string> query = null, string fragment = null)
        {

            if (string.IsNullOrWhiteSpace(baseUrl))
            {
                throw new ArgumentException("argument is not set", "BaseUrl");
            }

            BaseUrl = baseUrl;
            Path = path;

            if (fragment != null)
            {
                Fragment = fragment;
            }
            if (queryString != null)
            {
                QueryString = queryString;
            }
            if (query != null)
            {
                Query = query;
            }
        }

        public override string ToString()
        {
            var url = new StringBuilder();

            url.Append(BaseUrl.TrimEnd('/'));

            if (!string.IsNullOrWhiteSpace(Path))
            {
                url.Append($"/{Path.Trim('/')}");
            }

            if (Query != null && Query.Count > 0)
            {
                url.Append($"?{RenderQueryString()}");
            }

            if (!string.IsNullOrWhiteSpace(Fragment))
            {
                url.Append($"#{Fragment}");
            }

            return url.ToString();
        }

        public Uri ToUri()
        {
            return new Uri(ToString());
        }

        public string BaseUrl { get; set; }
        public string Path { get; set; }
        public string Fragment { get; set; }
        public Dictionary<string, string> Query { get; set; }
        public string QueryString
        {
            get
            {
                return RenderQueryString();
            }
            set
            {
                var query = ParseQueryString(value);

                if (query != null)
                {
                    Query = query;
                }
            }
        }

        private string RenderQueryString()
        {
            if (Query != null && Query.Count > 0)
            {
                return string.Join("&", Query.Select(entry => $"{entry.Key}={Uri.EscapeUriString(entry.Value)}"));
            }

            return null;
        }

        private Dictionary<string, string> ParseQueryString(string queryString)
        {
            if (!string.IsNullOrEmpty(queryString))
            {
                return queryString.Split('&')
                    .ToDictionary(parameter => parameter.Split('=')[0],
                                  parameter => Uri.UnescapeDataString(parameter.Split('=')[1]));
            }

            return null;
        }
    }
}
