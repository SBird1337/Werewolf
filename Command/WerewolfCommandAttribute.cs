namespace Werewolf.Command
{
    [System.AttributeUsage(System.AttributeTargets.Method, Inherited = false, AllowMultiple = true)]
    public sealed class WerewolfCommandAttribute : System.Attribute
    {
        readonly string _friendlyName;
        
        public WerewolfCommandAttribute(string friendlyName)
        {
            _friendlyName = friendlyName;
        }
        
        public string PositionalString
        {
            get { return _friendlyName; }
        }
    }
}