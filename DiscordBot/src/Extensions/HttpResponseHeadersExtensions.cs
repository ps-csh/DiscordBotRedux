using System.Net.Http.Headers;

namespace DiscordBot.Extensions
{
    /// <summary>
    /// Convience extensions for getting specific values from HttpResponseHeaders
    /// </summary>
    internal static class HttpResponseHeadersExtensions
    {
        /// <summary>
        /// Gets this first header matching a key from an HttpResponseHeaders collection
        /// </summary>
        /// <param name="responseHeaders">HttpResponseHeaders object to extend</param>
        /// <param name="header">The header (key) to get from header collection</param>
        /// <returns>A single header value</returns>
        /// <exception cref="ArgumentNullException"/>
        /// <exception cref="InvalidOperationException"/>
        public static string? GetHeader(this HttpResponseHeaders responseHeaders, string header)
        {
            return responseHeaders.TryGetValues(header, out var value) ? value.First() : null;
        }

        /// <summary>
        /// Gets this first header matching a key from an HttpResponseHeaders collection, and
        /// converts it to an integer
        /// </summary>
        /// <param name="responseHeaders">HttpResponseHeaders object to extend</param>
        /// <param name="header">The header (key) to get from header collection</param>
        /// <returns>A single header value as integer</returns>
        /// <exception cref="ArgumentNullException"/>
        /// <exception cref="InvalidOperationException"/>
        /// <exception cref="FormatException"/>
        /// <exception cref="OverflowException"/>
        public static int? GetHeaderAsInt(this HttpResponseHeaders responseHeaders, string header)
        {
            return responseHeaders.TryGetValues(header, out var value) ? int.Parse(value.First()) : null;
        }
    }
}
