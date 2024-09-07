using DiscordBot.DiscordAPI.Structures;
using DiscordBot.Logging;
using DiscordBot.Startup.Configuration;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
using System.Net;

namespace DiscordBot.DiscordAPI
{
    internal class DiscordAPIClient
    {
        private const int MESSAGE_CHARACTER_LIMIT = 2000;
        private const string JSON_CONTENT_TYPE = "application/json";
        private const string MULTIPART_CONTENT_TYPE = "multipart/form-data";

        private readonly ILogger _logger;
        //TODO: Should this be stored?
        private readonly DiscordApiEndpointsConfiguration _endpoints;
        private readonly DiscordHttpRequestHandler _httpRequestHandler;

        public DiscordAPIClient(IOptions<DiscordApiEndpointsConfiguration> endpoints,
            DiscordHttpRequestHandler httpRequestHandler, ILogger logger)
        {
            _logger = logger;
            _endpoints = endpoints.Value;
            _httpRequestHandler = httpRequestHandler;
        }

        public async Task<HttpResponseMessage?> PostMessage(string message, string channel)
        {
            try
            {
                string channelUrl = string.Format(_endpoints.Message, channel);

                if (message.Length > MESSAGE_CHARACTER_LIMIT)
                {
                    List<string>? separatedMessages = SplitMessage(message, MESSAGE_CHARACTER_LIMIT);
                    if (separatedMessages != null)
                    {
                        var response = await SendMultiple(separatedMessages, channelUrl);
                        return response;
                    }
                }
                else
                {
                    DiscordSendMessageStructure postMessage = new(message)
                    {
                        Tts = false
                    };

                    var response = await SendPlainMessage(postMessage.ToJson()!, channelUrl);

                    return response;
                }
            }
            catch (Exception ex)
            {
                _logger.LogException(ex, $"Failed to PostMessage to {channel}");
            }

            return null;
        }

        public async Task<HttpResponseMessage?> PostEmbedMessage(DiscordEmbedObjectStructure embed, string channel)
        {
            try
            {
                string channelUrl = string.Format(_endpoints.Message, channel);

                if (!embed.IsValidRichEmbed)
                {
                    List<DiscordEmbedObjectStructure>? separatedEmbeds = SplitEmbed(embed);
                    if (separatedEmbeds != null)
                    {
                        var sendMessages = separatedEmbeds.Select(e => new DiscordSendMessageStructure(e)
                        {
                            Tts = false
                        });
                        var response = await SendMultiple(sendMessages, channelUrl);
                        return response;
                    }
                }
                else
                {
                    DiscordSendMessageStructure postMessage = new(embed)
                    {
                        Tts = false
                    };

                    var response = await SendMessage(postMessage, channelUrl);

                    return response;
                }
            }
            catch (Exception ex)
            {
                _logger.LogException(ex, $"Failed to PostMessage to {channel}");
            }

            return null;
        }

        //TODO: Adjust function to split embeds
        public async Task<HttpResponseMessage?> PostEmbedMessage(DiscordEmbedObjectStructure[] embed, string channel)
        {
            try
            {
                string channelUrl = string.Format(_endpoints.Message, channel);

                //if (!embed.IsValidRichEmbed)
                //{
                //    List<DiscordEmbedObjectStructure>? separatedEmbeds = SplitEmbed(embed);
                //    if (separatedEmbeds != null)
                //    {
                //        var sendMessages = separatedEmbeds.Select(e => new DiscordSendMessageStructure(e)
                //        {
                //            Tts = false
                //        });
                //        var response = await SendMultiple(sendMessages, channelUrl);
                //        return response;
                //    }
                //}
                //else
                {
                    foreach (var embedObject in embed)
                    {
                        _logger.LogInfo($"Embed {embedObject.Title}, {embedObject.TotalRichEmbedCharacters} characters");
                    }

                    DiscordSendMessageStructure postMessage = new(embed)
                    {
                        Tts = false
                    };

                    var response = await SendMessage(postMessage, channelUrl);

                    return response;
                }
            }
            catch (Exception ex)
            {
                _logger.LogException(ex, $"Failed to PostMessage to {channel}");
            }

            return null;
        }

        private async Task<HttpResponseMessage?> SendPlainMessage(string message, string endpoint)
        {
            _logger.LogDebug("Sending plain message: " + message);

            var response = await _httpRequestHandler.PostRequest(endpoint, message, JSON_CONTENT_TYPE);

            return response;
        }

        private async Task<HttpResponseMessage?> SendMessage(DiscordSendMessageStructure message, string endpoint)
        {
            _logger.LogDebug("Sending message: " + message.ToString());

            var response = await _httpRequestHandler.PostRequest(endpoint, message.ToJson(), JSON_CONTENT_TYPE);

            return response;
        }

        private async Task<HttpResponseMessage?> SendMultiple(IEnumerable<string> messages, string channelUrl)
        {
            HttpResponseMessage? lastResult = null;
            foreach (var message in messages)
            {
                var postMessage = new DiscordSendMessageStructure(message);
                postMessage.SetNonce();

                lastResult = await SendPlainMessage(postMessage.ToJson(), channelUrl);
                if (lastResult != null && lastResult.StatusCode == HttpStatusCode.OK)
                {
                    try
                    {
                        //TODO: Restructure so response message is fully null checked
                        if (lastResult.Content != null)
                        {
                            var responseMessage = JObject.Parse(lastResult.Content.ToString()!).ToObject<DiscordMessageObjectStructure>();
                            if (responseMessage?.Nonce != postMessage.Nonce)
                            {
                                _logger.LogWarning($"Response contained a different Nonce value\n" +
                                    $"{responseMessage?.ToJson()}");
                                break;
                            }
                        }
                        else
                        {
                            _logger.LogWarning("Response content was null");
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogException(ex);
                    }
                }
                else
                {
                    _logger.LogWarning($"Did not receive Http response 200 (OK) when sending separated messages\n" +
                        $"{lastResult?.StatusCode}: {lastResult?.Content.ToString()}");
                    return lastResult;
                }

                _logger.LogInfo(lastResult?.Content?.ToString() ?? "Content was null");
            }

            return lastResult;
        }

        //TODO: Try to avoid repeat code with above method
        private async Task<HttpResponseMessage?> SendMultiple(IEnumerable<DiscordSendMessageStructure> messages, string channelUrl)
        {
            HttpResponseMessage? lastResult = null;
            foreach (var message in messages)
            {
                message.SetNonce();

                lastResult = await SendMessage(message, channelUrl);
                if (lastResult != null && lastResult.StatusCode == HttpStatusCode.OK)
                {
                    try
                    {
                        //TODO: Restructure so response message is fully null checked
                        if (lastResult.Content != null)
                        {
                            var responseMessage = JObject.Parse(lastResult.Content.ToString()!).ToObject<DiscordMessageObjectStructure>();
                            if (responseMessage?.Nonce != message.Nonce)
                            {
                                _logger.LogWarning($"Response contained a different Nonce value\n" +
                                    $"{responseMessage?.ToJson()}");
                                break;
                            }
                        }
                        else
                        {
                            _logger.LogWarning("Response content was null");
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogException(ex);
                    }
                }
                else
                {
                    _logger.LogWarning($"Did not receive Http response 200 (OK) when sending separated messages\n" +
                        $"{lastResult?.StatusCode}: {lastResult?.Content.ToString()}");
                    return lastResult;
                }

                _logger.LogInfo(lastResult?.Content?.ToString() ?? "Content was null");
            }

            return lastResult;
        }

        private List<string>? SplitMessage(string message, int maxCharacters)
        {
            try
            {
                List<string> separatedMessages = [];
                while (message.Length > maxCharacters)
                {
                    string currentMessage = message[..maxCharacters];
                    // Attempt to split the message from the last found space or newline in the substring
                    int lastSpaceIndex = currentMessage.LastIndexOfAny(['\n', ' ']);
                    if (lastSpaceIndex > 0)
                    {
                        currentMessage = currentMessage[..lastSpaceIndex];
                    }
                    separatedMessages.Add(currentMessage);
                    message = message.Remove(0, currentMessage.Length);
                }
                // Add the remaining string. Check for zero in case the message was split at the exact maxCharacters limit
                if (message.Length > 0) { separatedMessages.Add(message); }

                return separatedMessages;
            }
            catch (Exception ex)
            {
                _logger.LogException(ex, $"Failed to split message: {message}");
            }

            return null;
        }

        /// <summary>
        /// Attempts to break an embed into several smaller embeds
        /// </summary>
        /// <param name="embed"></param>
        /// <returns></returns>
        private List<DiscordEmbedObjectStructure> SplitEmbed(DiscordEmbedObjectStructure embed)
        {
            List<DiscordEmbedObjectStructure> embeds = [];

            DiscordEmbedObjectStructure firstEmbed = new DiscordEmbedObjectStructure()
            {
                Title = embed.Title,
                Description = embed.Description,
            }.SetFooter("...");

            for (int i = 0; i < DiscordEmbedObjectStructure.MAX_FIELD_COUNT && embed.Fields.Count > 0; ++i)
            {
                var field = embed.Fields.First();
                // Break from this loop if the Field would cause the embed to exceed the character limit
                if (firstEmbed.TotalRichEmbedCharacters + field.Name.Length +
                    field.Value.Length > DiscordEmbedObjectStructure.MAX_RICH_EMBED_CHARACTERS)
                {
                    break;
                }
                firstEmbed.AddField(field.Name, field.Value, field.Inline);
                embed.Fields.RemoveAt(0);
            }
            embeds.Add(firstEmbed);

            while (!embed.IsValidRichEmbed)
            {
                var tempEmbed = new DiscordEmbedObjectStructure()
                {
                    Title = embed.Title,
                    Description = "...continued",
                }.SetFooter("...");

                for (int i = 0; i < DiscordEmbedObjectStructure.MAX_FIELD_COUNT; ++i)
                {
                    var field = embed.Fields.First();
                    // Break from this loop if the Field would cause the embed to exceed the character limit
                    if (tempEmbed.TotalRichEmbedCharacters + field.Name.Length +
                        field.Value.Length > DiscordEmbedObjectStructure.MAX_RICH_EMBED_CHARACTERS)
                    {
                        break;
                    }
                    tempEmbed.AddField(field.Name, field.Value, field.Inline);
                    embed.Fields.RemoveAt(0);
                }
                embeds.Add(tempEmbed);
            }

            embeds.Add(embed.SetDescription("...continued"));

            return embeds;
        }
    }
}
