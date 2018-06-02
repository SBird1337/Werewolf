using System;
using System.Collections;
using System.Collections.Generic;
using Discord;
using Discord.WebSocket;
using Discord.Commands;
using System.IO;
using Newtonsoft.Json;
using System.Reflection;
using System.Linq;
using Werewolf.Game.Roles;

namespace Werewolf.Game
{
    public class WerewolfManager
    {
        public Dictionary<SocketGuild, WerewolfGame> Games { get; set; }
        public Dictionary<SocketGuild, WerewolfSettings> Settings { get; set; }

        public IServiceProvider Services;

        public const string SETTINGS_FILE = "settings.json";

        private log4net.ILog _log = log4net.LogManager.GetLogger(typeof(WerewolfManager));

        public List<Type> PossibleRoles;

        public WerewolfManager()
        {
            Games = new Dictionary<SocketGuild, WerewolfGame>();
            Settings = new Dictionary<SocketGuild, WerewolfSettings>();
            PossibleRoles = Assembly.GetExecutingAssembly().GetTypes().Where(a => a.IsClass && !a.IsAbstract && !a.IsInterface && a.IsAssignableFrom(typeof(WerewolfRoleBase)) && a.Namespace != null && a.Namespace.Contains(@"Werewolf.Game.Roles")).ToList();
        }

        public void Serialize(string directory)
        {
            try
            {
                if (!Directory.Exists(directory))
                    Directory.CreateDirectory(directory);
                foreach (SocketGuild key in Settings.Keys)
                {
                    string idName = key.Id.ToString("X");
                    string configSavePath = Path.Combine(directory, idName);
                    _log.Info(configSavePath);
                    if (!Directory.Exists(configSavePath))
                        Directory.CreateDirectory(configSavePath);

                    JsonSerializer serializer = new JsonSerializer();
                    using (StreamWriter sw = new StreamWriter(Path.Combine(configSavePath, SETTINGS_FILE)))
                    using (JsonWriter writer = new JsonTextWriter(sw))
                    {
                        serializer.Serialize(writer, Settings[key]);
                    }
                }
            }
            catch (IOException ex)
            {
                _log.Error("Serialization failed.", ex);
            }
        }
        public WerewolfSettings GetOrCreateSettings(SocketGuild guild)
        {
            if (Settings.ContainsKey(guild))
                return Settings[guild];
            Settings.Add(guild, new WerewolfSettings());
            return Settings[guild];
        }
    }
}