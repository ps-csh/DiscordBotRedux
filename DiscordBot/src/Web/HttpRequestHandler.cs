using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace DiscordBot.Web
{
    internal class HttpRequestHandler : IHttpRequestHandler
    {
        protected HttpClient _httpClient;

        public HttpClient Client => _httpClient;

        public HttpRequestHandler()
        {

            //HttpRequestMessage httpRequestMessage = new HttpRequestMessage(HttpMethod.Post, _endpoints.MessageURL);
            HttpClientHandler httpClientHandler = new HttpClientHandler()
            {
                AutomaticDecompression = DecompressionMethods.GZip
            };
            _httpClient = new HttpClient(httpClientHandler);
        }

        Task<HttpResponseMessage?> IHttpRequestHandler.GetRequest(string uri, IEnumerable<KeyValuePair<string, string>> headers)
        {
            throw new NotImplementedException();
        }

        Task<HttpResponseMessage?> IHttpRequestHandler.GetRequest(string uri, Action<HttpRequestMessage>? webRequestBuilder)
        {
            throw new NotImplementedException();
        }

        Task<HttpResponseMessage?> IHttpRequestHandler.PostRequest(string uri, byte[] data, Action<HttpRequestMessage>? webRequestBuilder)
        {
            throw new NotImplementedException();
        }

        Task<HttpResponseMessage?> IHttpRequestHandler.PostRequest(string uri, string data, string contentType, Action<HttpRequestMessage>? webRequestBuilder)
        {
            throw new NotImplementedException();
        }

        Task<HttpResponseMessage?> IHttpRequestHandler.PostRequest(string uri, byte[] data, IEnumerable<KeyValuePair<string, string>> headers)
        {
            throw new NotImplementedException();
        }

        Task<HttpResponseMessage?> IHttpRequestHandler.PostRequest(string uri, string data, string contentType, IEnumerable<KeyValuePair<string, string>> headers)
        {
            throw new NotImplementedException();
        }
    }
}
