﻿using System;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;

namespace DiscordBot.Modules.Commands
{
    public class Commands : ModuleBase<SocketCommandContext>
    {
        /// <summary>
        /// Replies with pong
        /// </summary>
        /// <returns></returns>
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
                await Logger.LogException(e);
            }
        }

        /// <summary>
        /// Return the current version of the bot
        /// </summary>
        /// <returns></returns>
        [RequireUserPermission(GuildPermission.Administrator)]
        [Command("version")]
        [Summary("$version - returns the current bot version")]
        // Return the current version of the bot
        public async Task Version()
        {
            try
            {
                Embed embed = new EmbedBuilder
                    {
                        Title = "Version: " + DiscordBot.Version(),
                    }
                    .WithAuthor(Context.Client.CurrentUser)
                    .WithFooter(DiscordBot.Version())
                    .WithCurrentTimestamp()
                    .Build();

                await ReplyAsync(embed: embed);
            }
            catch (Exception e)
            {
                await Logger.LogException(e);
            }
        }

        /// <summary>
        /// Get the account information of a user
        /// </summary>
        /// <param name="user">user to return userinfo of</param>
        /// <returns></returns>
        [Command("userinfo")]
        [Summary("$userinfo {user/snowflake} - Shows userinfo")]
        public async Task Userinfo(SocketGuildUser user = null)
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
                    .AddField("Violation Count:", ViolationManager.CountUserViolations(user.Id))
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
                await Logger.LogException(e);
            }
        }

        /// <summary>
        /// Returns User Snowflake
        /// </summary>
        /// <param name="user">User from which to get Snowflake</param>
        /// <returns></returns>
        [Command("snowflake")]
        [Summary("$snowflake <user/snowflake> - returns the snowflake of the user")]
        public async Task Snowflake(SocketGuildUser user)
        {
            Embed embed = new EmbedBuilder
                {
                    Title = "Snowflake for user " + user.Username
                }.AddField("snowflake", user.Id)
                .Build();

            await ReplyAsync(embed: embed);
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
            foreach (CommandInfo command in DiscordBot.Commands.Commands)
            {
                embedBuilder.AddField(command.Name, command.Summary);
            }

            Embed helpEmbed = embedBuilder.Build();

            await ReplyAsync(embed: helpEmbed);
        }

        [RequireUserPermission(GuildPermission.Administrator, ErrorMessage = "You don't have permission to use this command")]
        [Command("Config")]
        public async Task Config()
        {
            EmbedBuilder embedBuilder = new EmbedBuilder()
                .WithTitle("Bot Config");

            var configOptions = DiscordBot.Config.GetChildren();

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

        [Command("GuildID")]
        public async Task guildID()
        {
            await ReplyAsync(DiscordBot.GuildId.ToString());
        }


    }
}