namespace Werewolf.Game
{
    public class WerewolfGame
    {
        public enum ManagerPhase {JOIN, PLAY};

        public ManagerPhase Phase {get;set;}

        public WerewolfGame()
        {
            Phase = ManagerPhase.JOIN;
        }
    }
}