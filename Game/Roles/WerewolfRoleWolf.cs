using System.Collections.Generic;
using Discord.Commands;
using Discord;

namespace Werewolf.Game.Roles
{
    [WerewolfRole(10)]
    public abstract class WerewolfRoleWolf : WerewolfRoleBase
    {
        protected WerewolfRoleWolf()
        {
        }
        public override string Name { get => "Werewolf";}

        public override async void OnRoleAssignment(IDMChannel privateChannel)
        {
            await privateChannel.SendMessageAsync("Your role is Werewolf!");
        }
    }
}