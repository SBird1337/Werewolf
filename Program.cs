using System;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using Discord.Commands;
using System.Xml;
using System.IO;
using System.Reflection;
using System.Collections.Generic;
using System.Collections;
using Werewolf.Game;
using Werewolf.Command;
using Microsoft.Extensions.DependencyInjection;


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
            _log.Info("Initialized log4net Logger");
            //Init Discord Client

            IServiceProvider services = ConfigureServices();

            _client = services.GetRequiredService<DiscordSocketClient>();

            _client.Ready += ReadyAsync;
            string token = Environment.GetEnvironmentVariable("DSC_TOKEN");
            _log.Info($"Authenticating with {token}");
            await _client.LoginAsync(TokenType.Bot, Environment.GetEnvironmentVariable("DSC_TOKEN"));
            await _client.StartAsync();

            await services.GetRequiredService<CommandHandlingService>().InitializeAsync();
            
            await Task.Delay(-1);
        }

        public Task ReadyAsync()
        {
            _log.Info("Client connected to Discord API");
            return Task.CompletedTask;
        }

        private IServiceProvider ConfigureServices()
        {
            return new ServiceCollection()
                .AddSingleton<DiscordSocketClient>()
                .AddSingleton<CommandService>()
                .AddSingleton<CommandHandlingService>()
                .AddSingleton<WerewolfManager>()
                .BuildServiceProvider();
        }
    }
}
