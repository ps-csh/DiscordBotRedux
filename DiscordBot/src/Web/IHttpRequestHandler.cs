using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace DiscordBot.Web
{
    internal interface IHttpRequestHandler
    {
        public HttpClient Client { get; }

        public Task<HttpResponseMessage?> GetRequest(string uri, IEnumerable<KeyValuePair<string, string>> headers);
        public Task<HttpResponseMessage?> GetRequest(string uri, Action<HttpRequestMessage>? webRequestBuilder = null);
        public Task<HttpResponseMessage?> PostRequest(string uri, byte[] data, Action<HttpRequestMessage>? webRequestBuilder = null);
        public Task<HttpResponseMessage?> PostRequest(string uri, string data, string contentType, Action<HttpRequestMessage>? webRequestBuilder = null);
        public Task<HttpResponseMessage?> PostRequest(string uri, byte[] data, IEnumerable<KeyValuePair<string, string>> headers);
        public Task<HttpResponseMessage?> PostRequest(string uri, string data, string contentType, IEnumerable<KeyValuePair<string, string>> headers);
    }
}
