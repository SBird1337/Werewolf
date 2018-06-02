using System;
using System.Collections;
using System.Collections.Generic;
using Discord;
using Discord.WebSocket;
using Discord.Commands;
using Werewolf.Game.Roles;

namespace Werewolf.Game
{
    public class WerewolfPlayer
    {
        public SocketUser User {get;set;}
        WerewolfRoleBase Role {get;set;}

        IDMChannel PrivateChannel {get;set;}
        public WerewolfPlayer(SocketUser user)
        {
            Role = null;
            User = user;
        }
    }
}