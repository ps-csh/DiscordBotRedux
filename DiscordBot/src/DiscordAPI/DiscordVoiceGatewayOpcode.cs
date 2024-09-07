using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordBot.DiscordAPI
{
    /// <summary>
    /// 
    /// </summary>
    /// <remarks>
    /// <see href="https://discord.com/developers/docs/topics/opcodes-and-status-codes#voice"/>
    /// </remarks>
    internal enum DiscordVoiceGatewayOpcode
    {
        Identify, //client
        SelectProtocol, //client
        Ready, //server
        Heartbeat, //client
        SessionDescription, //server
        Speaking, //client/server
        HeartbeatACK, //server
        Resume, //client
        Hello, //server
        Resumed, //server
        ClientDisconnect = 13 //server
    }
}
