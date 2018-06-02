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
using Newtonsoft.Json;

namespace Werewolf
{
    class Program
    {
        private log4net.ILog _log = log4net.LogManager.GetLogger(typeof(Program));
        private IServiceProvider _services;

        private const string DATA_DIR = "serialization/";

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

            //Init Exit Handler
            AppDomain.CurrentDomain.ProcessExit += OnProcessExit;
            Console.CancelKeyPress += OnProcessExit;

            //Init Discord Client

            _services = ConfigureServices();

            DiscordSocketClient client = _services.GetRequiredService<DiscordSocketClient>();

            client.Ready += ReadyAsync;
            string token = Environment.GetEnvironmentVariable("DSC_TOKEN");
            _log.Info($"Authenticating with {token}");

            try
            {
                await client.LoginAsync(TokenType.Bot, Environment.GetEnvironmentVariable("DSC_TOKEN"));
                await client.StartAsync();
            }
            catch (Discord.Net.HttpException ex)
            {
                _log.Error("Could not connect to the discord bot.", ex);
                return;
            }


            await _services.GetRequiredService<CommandHandlingService>().InitializeAsync();

            await Task.Delay(-1);
        }

        private void OnProcessExit(object sender, EventArgs e)
        {
            _services.GetRequiredService<WerewolfManager>().Serialize(DATA_DIR);
        }

        public Task ReadyAsync()
        {
            _log.Info("Client connected to Discord API");
            Deserialize(DATA_DIR);
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

        private void Deserialize(string directory)
        {
            if (!Directory.Exists(directory))
                return;

            DiscordSocketClient client = _services.GetRequiredService<DiscordSocketClient>();
            WerewolfManager manager = _services.GetRequiredService<WerewolfManager>();
            foreach (string dir in Directory.GetDirectories(directory))
            {
                ulong guildId = Convert.ToUInt64(Path.GetFileName(dir), 16);

                SocketGuild guild = client.GetGuild(guildId);
                JsonSerializer serializer = new JsonSerializer();
                try
                {
                    using (StreamReader sr = new StreamReader(Path.Combine(dir, WerewolfManager.SETTINGS_FILE)))
                    using (JsonReader jr = new JsonTextReader(sr))
                    {
                        WerewolfSettings loadedSettings = serializer.Deserialize<WerewolfSettings>(jr);
                        loadedSettings.Restore(guild);
                        manager.Settings.Add(guild, loadedSettings);
                    }
                }
                catch (IOException ex)
                {
                    _log.Error("Could not load Settings", ex);
                }

            }
        }
    }
}
