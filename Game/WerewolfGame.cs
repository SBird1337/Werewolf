using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Discord.WebSocket;

namespace Werewolf.Game
{
    public class WerewolfGame
    {
        public enum ManagerPhase {JOIN, PLAY};
        public enum GamePhases {ROLE_ASSIGN, NIGHT_TARGET, NIGHT_EXEC, DAY_VOTE, DAY_EXEC, DAY_END, GAME_RESULT}

        public int NightCount {get;set;}
        public int DayCount {get;set;}

        public ManagerPhase Phase {get;set;}

        public GamePhases GamePhase {get;set;}

        public List<WerewolfPlayer> Players {get;set;}

        public WerewolfGame()
        {
            Phase = ManagerPhase.JOIN;
            Players = new List<WerewolfPlayer>();
        }

        public async void DoGame(WerewolfSettings settings, SocketGuild guild)
        {
            await settings.Channel.SendMessageAsync($"The game has started, players: {string.Join(", ", Players.Select(p => p.User.Username))}");
            await Task.Delay(10000);
            GamePhase = GamePhases.ROLE_ASSIGN;
            NightCount = 1;
            DayCount = 0;
        }
    }
}