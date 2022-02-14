using System;
using System.Threading.Tasks;
using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using DiscordBot.Database;
using DiscordBot.Objects.Embeds;
using DiscordBot.Objects.Embeds.Member;

namespace DiscordBot.Modules.Commands.TextCommands
{
    public class Commands : InteractionModuleBase<ShardedInteractionContext>
    {
        public DatabaseService DatabaseService { get; set; }

        #region User Commands

        /// <summary>
        /// Replies with pong
        /// </summary>
        /// <returns></returns>
        [SlashCommand("ping", "Check the bot status")]
        public async Task Ping()
        {
            await DeferAsync();

            try
            {
                await FollowupAsync("Pong");
            }
            catch (Exception e)
            {
                await EventHandlers.LogException(e, Context.Guild);
            }
        }

        #endregion

        #region Moderation Commands

        /// <summary>
        /// Command for getting the account information for a user
        /// </summary>
        /// <param name="user">user to return userinfo of</param>
        /// <returns></returns>
        [SlashCommand("userinfo", "Check information on the specified user")]
        public async Task UserInfo(SocketGuildUser user = null)
        {
            Embed embed;
            try
            {
                user ??= (SocketGuildUser)Context.User;

                embed = user.GuildPermissions.ModerateMembers
                    ? new UserInfo(user, Context.Client, DatabaseService, true).Build()
                    : new UserInfo(user, Context.Client, DatabaseService).Build();

                await RespondAsync(embed: embed);
            }
            catch (NullReferenceException)
            {
                embed = new EmbedBuilder
                    {
                        Title = "Missing Username/Snowflake"
                    }
                    .AddField("Example", "$userinfo [username/snowflake]")
                    .Build();
                await RespondAsync(embed: embed);
            }
            catch (Exception e)
            {
                await EventHandlers.LogException(e, Context.Guild);
            }
        }
        
        /// <summary>
        /// Command for purging the specified amount of messages
        /// </summary>
        /// <param name="amount"></param>
        [SlashCommand("purge", "Delete the specified amount of messages")]
        [RequireUserPermission(GuildPermission.ManageMessages)]
        public async Task Purge(int amount)
        {
            var messageList = await Context.Channel.GetMessagesAsync(amount + 1).FlattenAsync();

            await (Context.Channel as SocketTextChannel).DeleteMessagesAsync(messageList);

            await Task.CompletedTask;
        }

        #endregion

        #region Administration Commands

        /// <summary>
        /// Return the current version of the bot
        /// </summary>
        /// <returns></returns>
        [RequireUserPermission(GuildPermission.ManageGuild)]
        [SlashCommand("version", "Returns the version of the bot")]
        public async Task Version()
        {
            try
            {
                Embed embed = new BotVersion(Context).Build();

                await RespondAsync(embed: embed);
            }
            catch (Exception e)
            {
                await EventHandlers.LogException(e, Context.Guild);
            }
        }

        /// <summary>
        /// Command for initializing the bot if it was not done autmatically
        /// </summary>
        [RequireUserPermission(GuildPermission.ManageGuild)]
        [SlashCommand("init-bot", "Initializes the basic bot settings for this guild")]
        public async Task SetupBot()
        {
            try
            {
                JoinedGuild.AddGuild(Context.Guild, DiscordBot.Services);
                JoinedGuild.DownloadMembers(Context.Guild.Users, Context.Guild.Id, DiscordBot.Services);
                JoinedGuild.SetGuildOwner(Context.Guild.OwnerId, Context.Guild.Id, DiscordBot.Services);
                JoinedGuild.GenerateDefaultViolation(Context.Guild, DiscordBot.Services);
                await RespondAsync("Succesfully initialized the bot");
            }
            catch (Exception ex)
            {
                await RespondAsync("Failed to initialize bot");
            }
        }

        #endregion

        /// <summary>
        /// Comand for returing basic information on the server
        /// </summary>
        [SlashCommand("server-info", "Returns a basic set of information off the guild")]
        public async Task ServerInfo()
        {
            Embed embed = new ServerInfo(Context.Guild).Build();
            await RespondAsync(embed: embed);
        }
    }
}