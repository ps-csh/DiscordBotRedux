namespace DiscordBot.DiscordAPI
{
    /// <summary>
    /// Discord Gateway Opcodes representing the type of message
    /// </summary>
    /// <remarks>
    /// <see href="https://discord.com/developers/docs/topics/opcodes-and-status-codes#gateway"/>
    /// </remarks>
    public enum DiscordGatewayOpcode : int
    {
                                //Event is sent by client or received by client (from API)
        Dispatch = 0,           //Recieve
        Heartbeat,              //Send/Receive
        Identify,               //Send
        PresenceUpdate,         //Send
        VoiceStateUpdate,       //Send, Receive
        //OpCode 5 is not used by Discord API
        Resume = 6,             //Send
        Reconnect,              //Receive
        RequestGuildMembers,    //Send
        InvalidSession,         //Receive
        Hello,                  //Receive
        HeartbeatAcknowledge    //Receive
    }
}