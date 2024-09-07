using DiscordBot.DiscordAPI;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordBot.Web
{
    //TODO: Remove this class if I can't find a use for it
    internal class HttpRequestHandlerFactory
    {
        private Dictionary<string, IHttpRequestHandler> _handlers = [];

        public HttpRequestHandlerFactory(IServiceProvider serviceProvider)
        {
            var d = serviceProvider.GetService<DiscordHttpRequestHandler>();
            var h = serviceProvider.GetService<HttpRequestHandler>();

            _handlers.Add("Discord", d);
            _handlers.Add("Default", h);
        }
    }
}
