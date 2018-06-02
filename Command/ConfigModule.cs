using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Werewolf.Game;
using static Werewolf.Game.WerewolfGame;

namespace Werewolf.Command
{
    // Modules must be public and inherit from an IModuleBase
    public class ConfigModule : ModuleBase<SocketCommandContext>
    {
        public WerewolfManager WerewolfManager { get; set; }
        private log4net.ILog _log = log4net.LogManager.GetLogger(typeof(ConfigModule));

        [Command("ping")]
        [Alias("pong", "hello")]
        public Task PingAsync() => ReplyAsync("pong!");

        [Command("werewolf")]
        [RequireContext(ContextType.Guild)]
        public async Task WerewolfConfigAsync(params string[] args)
        {
            if (args.Length == 0)
            {
                await ReplyAsync("Command `werewolf` needs to be followed by a subcommand, possible subcommands: start, set.");
                return;
            }
            switch (args[0])
            {
                case "start":
                    if (args.Length != 1)
                    {
                        return;
                    }
                    if (WerewolfManager.Games.ContainsKey(Context.Guild))
                    {
                        await ReplyAsync($"The server `{Context.Guild.Name}` is already playing a game of werewolf.");
                    }
                    else
                    {
                        if (!WerewolfManager.Settings.ContainsKey(Context.Guild))
                        {
                            await ReplyAsync("You cannot start a game without specifying the Game Channel first, use `werewolf set #<channel>`");
                            return;
                        }
                        if (WerewolfManager.Settings[Context.Guild].Channel == null)
                        {
                            await ReplyAsync("You cannot start a game without specifying the Game Channel first, use `werewolf set #<channel>`");
                            return;
                        }
                        WerewolfManager.Games.Add(Context.Guild, new WerewolfGame());
                        await WerewolfManager.Settings[Context.Guild].Channel.SendMessageAsync($"{Context.User.Username} has started a new game of werewolf, use `!join` or `@werewolf join` to join in.");
                    }
                    break;
                case "set":
                    if (!((Context.User as SocketGuildUser).GuildPermissions.ManageChannels))
                    {
                        await ReplyAsync("Setting the Werewolf Channel requires permission `ManageChannels`.");
                        return;
                    }
                    if (args.Length < 2)
                    {
                        await ReplyAsync("What do you want to set? (`channel`)");
                        return;
                    }
                    else
                    {
                        switch (args[1])
                        {
                            case "channel":
                                if (args.Length != 3 || (Context.Message.MentionedChannels.FirstOrDefault() == null))
                                {
                                    await ReplyAsync("Usage: `werewolf set channel #<channel>`.");
                                    return;
                                }
                                WerewolfSettings settings = WerewolfManager.GetOrCreateSettings(Context.Guild);
                                SocketGuildChannel channel = Context.Message.MentionedChannels.FirstOrDefault();
                                if (!(channel is ISocketMessageChannel))
                                {
                                    //this should probably not even happen
                                    await ReplyAsync($"Specified channel `{channel.Name}` is not a Text Channel.");
                                    return;
                                }

                                settings.Channel = channel as ISocketMessageChannel;
                                await ReplyAsync($"Game Channel set to `{settings.Channel.Name}`");
                                break;
                        }
                    }
                    break;
                    case "begin":
                        if(args.Length != 1)
                            return;
                        if(!WerewolfManager.Games.ContainsKey(Context.Guild))
                        {
                            await ReplyAsync("Cannot begin the game, because none was started yet, use `werewolf start`.");
                            return;
                        }
                        if(WerewolfManager.Games[Context.Guild].Phase == ManagerPhase.JOIN)
                        {
                            WerewolfManager.Games[Context.Guild].Phase = ManagerPhase.PLAY;

                            //Start async game while still listening for commands
                            WerewolfManager.Games[Context.Guild].DoGame(WerewolfManager.Settings[Context.Guild], Context.Guild);
                        }
                    break;
            }
        }

        [Command("join")]
        public async Task WerewolfJoinAsync()
        {
            if (WerewolfManager.Games.ContainsKey(Context.Guild) && WerewolfManager.Games[Context.Guild].Phase == ManagerPhase.JOIN)
            {
                //TODO: Check for max players and things
                bool joined = true;
                if (WerewolfManager.Games[Context.Guild].Players.Any(p => p.User == Context.User))
                {
                    WerewolfManager.Games[Context.Guild].Players.RemoveAll(p => p.User == Context.User);
                    joined = false;
                }
                else
                {
                    try
                    {
                        await Context.User.SendMessageAsync("You have joined the game and are ready to go, it will begin shortly. Once it begins you can use your abilities from here and will get your personal information from me.");

                    }
                    catch (Discord.Net.HttpException)
                    {
                        await ReplyAsync($"{Context.User.Mention}: I could not send you a private message, which is required in order to play `Werewolf`. Please change your privacy settings accordingly.");
                        return;
                    }
                    WerewolfManager.Games[Context.Guild].Players.Add(new WerewolfPlayer(Context.User));
                }

                int pc = WerewolfManager.Games[Context.Guild].Players.Count;
                await WerewolfManager.Settings[Context.Guild].Channel.SendMessageAsync($"{Context.User.Mention} {(joined ? "joined" : "left")} the game, we currently have {pc.ToString()} player{(pc != 1 ? "s" : "")}{(pc > 0 ? ": " : "")}{String.Join(", ", WerewolfManager.Games[Context.Guild].Players.Select(p => p.User.Username))}.");
            }
        }

    }
}