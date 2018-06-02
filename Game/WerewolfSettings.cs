using System;
using System.Collections;
using System.Collections.Generic;
using Discord;
using Discord.WebSocket;
using Discord.Commands;
using Newtonsoft.Json;

namespace Werewolf.Game
{
    public class WerewolfSettings
    {
        [JsonIgnore]
        private ISocketMessageChannel _channel;
        public string ChannelId {get;set;}

        [JsonIgnore]
        public ISocketMessageChannel Channel {
            get
            {
                return _channel;
            }
            set
            {
                _channel = value;
                ChannelId = _channel.Id.ToString("X");
            }
        }
        public WerewolfSettings()
        {
        }

        public void Restore(SocketGuild guild)
        {
            ulong id = Convert.ToUInt64(ChannelId, 16);
            if(guild == null)
                throw new ArgumentNullException("guild");
            SocketGuildChannel channel = guild.GetChannel(id);
            if(!(channel is ISocketMessageChannel))
                throw new ArgumentException();
            Channel = guild.GetChannel(id) as ISocketMessageChannel;
        }
    }
}