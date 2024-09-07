using System;
using System.Collections.Generic;
using System.Text;

namespace DiscordBot.Database.Models
{
    public class Quote : PostableObject
    {
        public string Message { get; set; }
    }
}
