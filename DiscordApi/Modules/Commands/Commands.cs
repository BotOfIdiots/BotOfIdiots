using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using DiscordBot.Database;
using DiscordBot.DiscordApi;
using DiscordBot.DiscordApi.Modules;

namespace DiscordBot.Modules.Commands
{
    public class Commands : ModuleBase<SocketCommandContext>
    {
        #region User Commands

        /// <summary>
        /// Replies with pong
        /// </summary>
        /// <returns>
        /// Returns a message to the channel were the command is issued
        /// </returns>
        [Command("ping")]
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

        /// <summary>
        /// View embed with all bot commands
        /// </summary>
        /// <returns>
        /// Embed with all bot commands to the channel were the command is issued
        /// </returns>
        [Command("help")]
        [Summary("$help - returns a list of available commands")]
        public async Task Help()
        {
            EmbedBuilder embedBuilder = new EmbedBuilder
            {
                Title = "Bot Commands",
                Color = Color.Teal
            };
            foreach (CommandInfo command in Bot.Commands.Commands)
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
        /// <returns>
        /// Embed with a users information sent in the channel were the command is issued
        /// </returns>
        [Command("userinfo")]
        [Summary("$userinfo {user/snowflake} - Shows userinfo")]
        [RequireUserPermission(GuildPermission.KickMembers)]
        public async Task UserInfo(SocketGuildUser user = null)
        {
            Embed embed;
            String roles = null;

            try
            {
                if (user == null)
                {
                    user = Context.Guild.GetUser(Context.User.Id);
                }

                foreach (IRole role in user.Roles.Distinct())
                {
                    if (roles == null)
                    {
                        roles = role.Mention;
                    }
                    else
                    {
                        roles += role.Mention;
                    }
                }

                embed = new EmbedBuilder { }
                    .AddField("User", user.Mention)
                    .WithThumbnailUrl(user.GetAvatarUrl())
                    // .AddField("Violation Count:", ViolationManager.CountUserViolations(user.Id))
                    .AddField("Created At", user.CreatedAt.ToString("dd-MM-yy HH:mm:ss"), true)
                    .AddField("Joined At", user.JoinedAt?.ToString("dd-MM-yy HH:mm:ss"), true)
                    .AddField("Roles", roles)
                    .WithAuthor(user)
                    .WithFooter("UserID: " + user.Id)
                    .WithCurrentTimestamp()
                    .Build();

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
        /// Delete certain amount of messages at once
        /// </summary>
        /// <param name="amount"></param>
        /// <returns>
        /// Deletes user specified amount of messages were the command is issued
        /// </returns>
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
        /// <returns>
        /// Embed with the current bot version sent in the channel were the command is issued
        /// </returns>
        [RequireUserPermission(GuildPermission.Administrator, ErrorMessage =
        "You don't have permission to use this command")]

        [Command("version")]
        [Summary("$version - returns the current bot version")]
        // Return the current version of the bot
        public async Task Version()
        {
            try
            {
                Embed embed = new EmbedBuilder
                    {
                        Title = "Version: " + Base.Version(),
                    }
                    .WithAuthor(Context.Client.CurrentUser)
                    .WithFooter(Base.Version())
                    .WithCurrentTimestamp()
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
        [RequireUserPermission(GuildPermission.Administrator,
            ErrorMessage = "You don't have permission to use this command")]
        [Command("Config")]
        public async Task Config()
        {
            EmbedBuilder embedBuilder = new EmbedBuilder()
                .WithTitle("Bot Config");

            var configOptions = Bot.Config.GetChildren();

            foreach (var option in configOptions)
            {
                if (option.GetChildren().Any())
                {
                    var section = option.GetChildren();

                    foreach (var suboption in section)
                    {
                        embedBuilder.AddField(suboption.Key, suboption.Value);
                    }
                }
                else
                {
                    if (option.Key != "Token")
                    {
                        embedBuilder.AddField(option.Key, option.Value);
                    }
                }
            }

            Embed embed = embedBuilder.Build();

            await ReplyAsync(embed: embed);
        }

        /// <summary>
        /// If automatic setup failed, issue this manual command
        /// </summary>
        /// <summary>
        /// Sets up the bot for a new server
        /// </summary>
        [RequireUserPermission(GuildPermission.ManageGuild, ErrorMessage =
            "You don't have permission to use this command")]
        [Command("setupbot")]
        public async Task SetupBot()
        {
            JoinedGuild.AddGuild(Context.Guild);
            JoinedGuild.DownloadMembers(Context.Guild.Users, Context.Guild.Id);
            JoinedGuild.SetGuildOwner(Context.Guild.OwnerId, Context.Guild.Id);

            await Task.CompletedTask;
        }

        #endregion
    }
}