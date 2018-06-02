using System;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using System.Xml;
using System.IO;
using System.Reflection;



namespace Werewolf
{
    class Program
    {
        private DiscordSocketClient _client;
        private log4net.ILog _log = log4net.LogManager.GetLogger(typeof(Program));
        static void Main(string[] args)
            => new Program().MainAsync().GetAwaiter().GetResult();

        public async Task MainAsync()
        {
            
            //Init Logger
            XmlDocument log4netConfig = new XmlDocument();
            log4netConfig.Load(File.OpenRead("log4net.config"));

            var repo = log4net.LogManager.CreateRepository(
                Assembly.GetEntryAssembly(), typeof(log4net.Repository.Hierarchy.Hierarchy));
            
            log4net.Config.XmlConfigurator.Configure(repo, log4netConfig["log4net"]);

            //Init Discord Client
            _client = new DiscordSocketClient();

            _client.Ready += ReadyAsync;
            _client.MessageReceived += MessageReceivedAsync;
            string token = Environment.GetEnvironmentVariable("DSC_TOKEN");
            _log.Info($"Authenticating with {token}");
            await _client.LoginAsync(TokenType.Bot, Environment.GetEnvironmentVariable("DSC_TOKEN"));
            await _client.StartAsync();

            await Task.Delay(-1);
        }

        public Task ReadyAsync()
        {
            _log.Info("Client connected to Discord API");
            return Task.CompletedTask;
        }

        public Task MessageReceivedAsync(SocketMessage message)
        {
            _log.Info($"Message received: {message.Content}");
            return Task.CompletedTask;
        }
    }
}
