using System;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using DiscordBot.Database;
using DiscordBot.Objects.Embeds;
using DiscordBot.Objects.Embeds.Member;

namespace DiscordBot.Modules.Commands
{
    public class Commands : ModuleBase<ShardedCommandContext>
    {
        public DatabaseService DatabaseService { get; set;}
        public CommandService CommandService { get; set; }

        #region User Commands

        /// <summary>
        /// Replies with pong
        /// </summary>
        /// <returns></returns>
        [Command("ping")]
        [RequireUserPermission(GuildPermission.Administrator)]
        [Summary("$ping - Responds with Pong")]
        public async Task Ping()
        {
            try
            {
                await ReplyAsync("Pong");
            }
            catch (Exception e)
            {
                await EventHandlers.LogException(e, Context.Guild);
            }
        }

        [Command("help")]
        [Summary("$help - returns a list of available commands")]
        public async Task Help()
        {
            EmbedBuilder embedBuilder = new EmbedBuilder
            {
                Title = "Bot Commands",
                Color = Color.Teal
            };
            foreach (CommandInfo command in CommandService.Commands)
            {
                string summary;
                switch (command.Summary)
                {
                    case null:
                        summary = "Command doesn't have a description";
                        break;
                    default:
                        summary = command.Summary;
                        break;
                }

                embedBuilder.AddField(command.Name, summary);
            }

            Embed helpEmbed = embedBuilder.Build();

            await ReplyAsync(embed: helpEmbed);
        }

        #endregion

        #region Moderation Commands

        /// <summary>
        /// Get the account information of a user
        /// </summary>
        /// <param name="user">user to return userinfo of</param>
        /// <returns></returns>
        [Command("userinfo")]
        [Summary("$userinfo {user/snowflake} - Shows userinfo")]
        public async Task UserInfo(SocketGuildUser user = null)
        {
            Embed embed;
            try
            {
                user ??= Context.Guild.GetUser(Context.User.Id);

                if (user.GuildPermissions.KickMembers)
                {
                    embed = new UserInfo(user, Context.Client, DatabaseService, true).Build();
                }
                else
                {
                    embed = new UserInfo(user, Context.Client, DatabaseService).Build();
                }
                
                await ReplyAsync(embed: embed);
            }
            catch (NullReferenceException)
            {
                embed = new EmbedBuilder
                    {
                        Title = "Missing Username/Snowflake"
                    }
                    .AddField("Example", "$userinfo [username/snowflake]")
                    .Build();
                await ReplyAsync(embed: embed);
            }
            catch (Exception e)
            {
                await EventHandlers.LogException(e, Context.Guild);
            }
        }

        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="amount"></param>
        [Command("purge")]
        [Summary(
            "$purge <amount> - removes the amount of messages specified. (You don't have to count the command message as it is included by default")]
        [RequireUserPermission(GuildPermission.ManageMessages)]
        public async Task Purge(int amount)
        {
            var messageList = await Context.Channel.GetMessagesAsync(amount + 1).FlattenAsync();

            await (Context.Channel as SocketTextChannel).DeleteMessagesAsync(messageList);

            await Task.CompletedTask;
        }

        /// <summary>
        /// Returns User Snowflake
        /// </summary>
        /// <param name="user">User from which to get Snowflake</param>
        /// <returns></returns>
        [Command("snowflake")]
        [Summary("$snowflake <user/snowflake> - returns the snowflake of the user")]
        [RequireUserPermission(GuildPermission.KickMembers)]
        public async Task Snowflake(SocketGuildUser user)
        {
            Embed embed = new EmbedBuilder
                {
                    Title = "Snowflake for user " + user.Username
                }.AddField("snowflake", user.Id)
                .Build();

            await ReplyAsync(embed: embed);
        }

        #endregion

        #region Administration Commands

        /// <summary>
        /// Return the current version of the bot
        /// </summary>
        /// <returns></returns>
        [RequireUserPermission(GuildPermission.Administrator, ErrorMessage =
        "You don't have permission to use this command")]

        [Command("version")]
        [Summary("$version - returns the current bot version")]
        // Return the current version of the bot
        public async Task Version()
        {
            try
            {
                Embed embed = new BotVersion(Context).Build();

                await ReplyAsync(embed: embed);
            }
            catch (Exception e)
            {
                await EventHandlers.LogException(e, Context.Guild);
            }
        }

        [RequireUserPermission(GuildPermission.Administrator,
            ErrorMessage = "You don't have permission to use this command")]
        [Command("GuildID")]
        public async Task GuildId()
        {
            await ReplyAsync(Context.Guild.ToString());
        }

        [RequireUserPermission(GuildPermission.ManageGuild, ErrorMessage =
            "You don't have permission to use this command")]
        [Command("setupbot")]
        public async Task SetupBot()
        {
            JoinedGuild.AddGuild(Context.Guild, DiscordBot.Services);
            JoinedGuild.DownloadMembers(Context.Guild.Users, Context.Guild.Id, DiscordBot.Services);
            JoinedGuild.SetGuildOwner(Context.Guild.OwnerId, Context.Guild.Id, DiscordBot.Services);
            JoinedGuild.GenerateDefaultViolation(Context.Guild, DiscordBot.Services);

            await Task.CompletedTask;
        }
        #endregion

        [Command("serverinfo")]
        public async Task ServerInfo()
        {
            Embed embed = new ServerInfo(Context.Guild).Build();
            await ReplyAsync(embed: embed);
        }
    }
}