using System.Collections.Generic;
using Discord.Commands;
using Discord.WebSocket;
using Discord;

namespace Werewolf.Game.Roles
{
    public abstract class WerewolfRoleBase
    {
        public abstract string Name {get;}

        public abstract void OnRoleAssignment(IDMChannel privateChannel);
    }
}