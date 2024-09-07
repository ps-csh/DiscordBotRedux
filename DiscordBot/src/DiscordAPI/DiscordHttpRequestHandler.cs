using DiscordBot.Startup.Configuration;
using DiscordBot.Web;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using DiscordBot.Logging;
using System.Net.Http.Headers;
using DiscordBot.Extensions;
using static System.Runtime.InteropServices.JavaScript.JSType;
using Newtonsoft.Json.Linq;

namespace DiscordBot.DiscordAPI
{
    internal class DiscordHttpRequestHandler : IHttpRequestHandler
    {
        /**
         *  < HTTP/1.1 429 TOO MANY REQUESTS
            < Content-Type: application/json
            < Retry-After: 65
            < X-RateLimit-Limit: 10
            < X-RateLimit-Remaining: 0
            < X-RateLimit-Reset: 1470173023.123
            < X-RateLimit-Reset-After: 64.57
            < X-RateLimit-Bucket: abcd1234
            {
              "message": "You are being rate limited.",
              "retry_after": 64.57,
              "global": false
            }
         */

        private const string RATE_BUCKET_HEADER = "X-RateLimit-Bucket";
        private const string RATE_LIMIT_HEADER = "X-RateLimit-Limit";
        private const string RATE_REMAINING_HEADER = "X-RateLimit-Remaining";
        private const string RATE_RESET_HEADER = "X-RateLimit-Reset";
        private const string RATE_RESET_AFTER_HEADER = "X-RateLimit-Reset-After";

        private const string CHANNEL_ENDPOINT_REGEX = @"^/api/channel/(?<channelId>\d+)/$";

        private const int MAX_REQUEST_RETRIES = 3;

        public List<RateBucket> _rateBuckets;
        public RateBucket? _globalRateBucket;

        private bool globalRateLimit;
        private int globalRateReset;
        private Timer? globalResetTimer;

        private readonly ILogger _logger;
        private readonly HttpClient _httpClient;

        public HttpClient Client => _httpClient;

        public DiscordHttpRequestHandler(IOptions<DiscordApiEndpointsConfiguration> endpoint,
            IOptions<BotSecretsConfiguration> secrets, IOptions<BotSettingsConfiguration> settings, ILogger logger)
        {
            HttpClientHandler httpClientHandler = new()
            {
                AutomaticDecompression = DecompressionMethods.GZip,

            };

            string authorizationHeader = $"{secrets.Value.TokenType} {secrets.Value.Token}";
            string userAgentHeader = settings.Value.UserAgent;
            //var agent = new ProductInfoHeaderValue("DiscordBot", "0.1");

            _httpClient = new HttpClient(httpClientHandler);
            _httpClient.BaseAddress = new Uri(endpoint.Value.Base);
            _httpClient.DefaultRequestHeaders.Add("Authorization", authorizationHeader);
            _httpClient.DefaultRequestHeaders.Add("User-Agent", userAgentHeader);

            _logger = logger;
            _logger.LogInfo(_httpClient.DefaultRequestHeaders.UserAgent.First().ToString());

            _rateBuckets = [];
        }


        public async Task<HttpResponseMessage?> GetRequest(string uri, Action<HttpRequestMessage>? webRequestBuilder = null)
        {
            HttpRequestMessage requestMessage = new(HttpMethod.Get, uri);
            webRequestBuilder?.Invoke(requestMessage);
            return await SendRequest(uri, requestMessage);
        }

        public async Task<HttpResponseMessage?> GetRequest(string uri, IEnumerable<KeyValuePair<string, string>> headers)
        {
            HttpRequestMessage requestMessage = new(HttpMethod.Get, uri);
            foreach (var header in headers)
            {
                requestMessage.Headers.Add(header.Key, header.Value);
            }
            return await SendRequest(uri, requestMessage);
        }

        public async Task<HttpResponseMessage?> PostRequest(string uri, byte[] data, Action<HttpRequestMessage>? webRequestBuilder = null)
        {
            HttpRequestMessage requestMessage = new(HttpMethod.Post, uri)
            {
                Content = new ByteArrayContent(data)
            };

            webRequestBuilder?.Invoke(requestMessage);

            var response = await SendRequest(uri, requestMessage);
            return response;
        }

        public async Task<HttpResponseMessage?> PostRequest(string uri, string data, string contentType, Action<HttpRequestMessage>? webRequestBuilder = null)
        {
            HttpRequestMessage requestMessage = new(HttpMethod.Post, uri)
            {
                Content = new StringContent(data, null, contentType)
            };

            webRequestBuilder?.Invoke(requestMessage);

            var response = await SendRequest(uri, requestMessage);
            return response;
            //return await PostRequest(uri, Encoding.Default.GetBytes(data), webRequestBuilder);
        }

        public async Task<HttpResponseMessage?> PostRequest(string uri, byte[] data, IEnumerable<KeyValuePair<string, string>> headers)
        {
            HttpRequestMessage requestMessage = new(HttpMethod.Post, uri)
            {
                Content = new ByteArrayContent(data)
            };

            foreach (var header in headers)
            {
                requestMessage.Headers.Add(header.Key, header.Value);
            }

            var response = await SendRequest(uri, requestMessage);
            return response;
        }

        public async Task<HttpResponseMessage?> PostRequest(string uri, string data, string contentType, IEnumerable<KeyValuePair<string, string>> headers)
        {
            HttpRequestMessage requestMessage = new(HttpMethod.Post, uri)
            {
                Content = new StringContent(data, null, contentType)
            };

            foreach (var header in headers)
            {
                requestMessage.Headers.Add(header.Key, header.Value);
            }

            var response = await SendRequest(uri, requestMessage);
            return response;
        }

        private async Task<HttpResponseMessage?> SendRequest(string endpoint, HttpRequestMessage requestMessage, int retries = MAX_REQUEST_RETRIES)
        {
            _logger.LogDebug("PostRequest to " + _httpClient.BaseAddress + endpoint + ", " + retries);
            _logger.LogDebug($"Sending request: {requestMessage.Content}");
            HttpResponseMessage? response = null;
            try
            {
                if (retries <= 0)
                {
                    return new HttpResponseMessage(HttpStatusCode.BadRequest);
                }

                var bucket = GetRateBucket(endpoint);
                if (bucket?.Semaphore != null)
                {
                    await bucket.Semaphore.WaitAsync();
                }

                response = await _httpClient.SendAsync(requestMessage);
                SetRateBucket(response.Headers, endpoint);

                if (response.StatusCode == HttpStatusCode.TooManyRequests)
                {
                    return await SendRequest(endpoint, requestMessage, --retries);
                }
                else
                {
                    _logger.LogDebug("Response: " + response.StatusCode);
                }
            }
            catch (Exception ex)
            {
                _logger.LogException(ex);
            }

            return response;
        }


        private RateBucket? GetRateBucket(string endpoint)
        {
            //TEMP: Change back to FirstOrDefault or ensure that buckets get updated
            // if another bucket with different Id but same endpoint is created
            var bucket = _rateBuckets.SingleOrDefault(r => r.Endpoint == endpoint);
            return bucket;
        }

        private RateBucket? SetRateBucket(HttpResponseHeaders headers, string endpoint)
        {
            try
            {
                string id = headers.GetHeader(RATE_BUCKET_HEADER) ?? throw new ArgumentNullException(RATE_BUCKET_HEADER, "Header value was null");
                int requestLimit = headers.GetHeaderAsInt(RATE_LIMIT_HEADER) ?? throw new ArgumentNullException(RATE_LIMIT_HEADER, "Header value was null");
                int requestsRemaining = headers.GetHeaderAsInt(RATE_REMAINING_HEADER) ?? throw new ArgumentNullException(RATE_REMAINING_HEADER, "Header value was null");
                int resetAfter = headers.GetHeaderAsInt(RATE_RESET_AFTER_HEADER) ?? throw new ArgumentNullException(RATE_RESET_AFTER_HEADER, "Header value was null");

                var bucket = _rateBuckets.FirstOrDefault(r => r.Id == id);
                if (bucket == null)
                {
                    bucket = new RateBucket(id, endpoint, requestLimit, requestsRemaining, resetAfter);
                    _rateBuckets.Add(bucket);
                }
                else
                {
                    bucket.UpdateRates(requestsRemaining, resetAfter);
                }

                return bucket;
            }
            catch (Exception ex)
            {
                _logger.LogException(ex);
            }

            return null;
        }

        public class RateBucket
        {
            public string Id { get; set; }
            public string Endpoint { get; set; }
            public SemaphoreSlim Semaphore { get; set; }
            public Timer? ResetTimer { get; set; }
            public int RequestLimit { get; set; }
            public int RequestsRemaining { get; set; }

            public RateBucket(string id, string endpoint, int requestLimit, int requestsRemaining, int resetAfter)
            {
                Id = id;
                Endpoint = endpoint;
                RequestLimit = requestLimit;
                RequestsRemaining = requestsRemaining;
                Semaphore = new SemaphoreSlim(RequestsRemaining, RequestLimit);
                ResetTimer = new Timer(ReleaseSemaphores, Semaphore, resetAfter, Timeout.Infinite);
            }

            public void UpdateRates(int requestsRemaining, int resetAfter)
            {
                if (ResetTimer != null)
                {
                    RequestsRemaining = requestsRemaining;
                }
                else
                {
                    ResetTimer = new Timer(ReleaseSemaphores, Semaphore, resetAfter, Timeout.Infinite);
                }
            }

            private void ReleaseSemaphores(object? state)
            {
                try
                {
                    var semaphore = (SemaphoreSlim)state!;
                    semaphore.Release(RequestLimit - semaphore.CurrentCount);

                    RequestsRemaining = semaphore.CurrentCount;
                    ResetTimer?.Dispose();
                    ResetTimer = null;
                }
                catch (Exception)
                {
                    //TODO: Is there any error handling to do here?
                    // assuming it's called by an asynchronous Timer
                }
            }
        }
    }
}
