using System.Collections.Generic;
using Discord.Commands;

namespace Werewolf.Game.Roles
{
    [System.AttributeUsage(System.AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    sealed class WerewolfRoleAttribute : System.Attribute
    {
        public int AssignPriority {get;private set;}
        public WerewolfRoleAttribute(int assignPriority = 0)
        {
            AssignPriority = assignPriority;
        }

    }
}