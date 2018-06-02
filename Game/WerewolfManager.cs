using System;
using System.Collections;
using System.Collections.Generic;
using Discord;
using Discord.WebSocket;
using Discord.Commands;

namespace Werewolf.Game
{
    public class WerewolfManager
    {
        public Dictionary<SocketGuild, WerewolfGame> Games {get;set;}

        public WerewolfManager()
        {
            Games = new Dictionary<SocketGuild,WerewolfGame>();
        }
    }
}