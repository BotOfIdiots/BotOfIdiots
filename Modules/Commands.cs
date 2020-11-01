using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using DiscordBot.Models;

namespace DiscordBot.Modules
{
    public class Commands : ModuleBase<SocketCommandContext>
    {
        /// <summary>
        /// Replies with pong
        /// </summary>
        /// <returns></returns>
        [Command("ping")]
        public async Task Ping()
        {
            await ReplyAsync("Pong");
        }

        /// <summary>
        /// Return the current version of the bot
        /// </summary>
        /// <returns></returns>
        [Command("version")]
        // Return the current version of the bot
        public async Task Version()
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

        /// <summary>
        /// Get the account information of a user
        /// </summary>
        /// <param name="user">user to return userinfo of</param>
        /// <returns></returns>
        [Command("userinfo")]
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
                Console.WriteLine(e);
            }
        }

        /// <summary>
        /// Get the violations of a user
        /// </summary>
        /// <param name="guildUser"></param>
        /// <returns></returns>
        [Command("violations")]
        public async Task Violations(SocketGuildUser guildUser)
        {
            List<Violation> violations = ViolationManager.GetViolations(guildUser.Id);

            EmbedBuilder embedBuilder = new EmbedBuilder();
            embedBuilder.WithTitle("Violations")
                .AddField("User:", guildUser.Mention)
                .AddField("ViolationCount", violations.Count)
                .WithCurrentTimestamp()
                .WithFooter("UserID: " + guildUser.Id)
                ;

            foreach (Violation violation in violations)
            {
                string violationType;
                switch (violation.Type)
                {
                    case 1:
                        violationType = "Ban";
                        break;
                    case 2:
                        violationType = "Kick";
                        break;
                    case 3:
                        violationType = "Mute";
                        break;
                    case 4:
                        violationType = "Unmute";
                        break;
                    default:
                        violationType = "Warn";
                        break;
                }

                embedBuilder.AddField("Violation", violation.Id + " - " + violationType);
            }

            Embed embed = embedBuilder.Build();

            await ReplyAsync(embed: embed);
        }

        /// <summary>
        /// Get a specific violation
        /// </summary>
        /// <param name="violationId"></param>
        /// <returns></returns>
        [Command("violation")]
        public async Task Violation(int violationId)
        {
            Embed embed = ViolationManager.GetViolation(violationId, Context);

            await ReplyAsync(embed: embed);
        }

        /// <summary>
        /// Kick a user
        /// </summary>
        /// <param name="kickedUser">User to kick</param>
        /// <param name="reason" default="No reason specified">Reason for the kick</param>
        /// <returns></returns>
        [Command("kick")]
        public async Task Kick(SocketGuildUser kickedUser, [Remainder] string reason = "No reason specified.")
        {
            Embed embed;
            if (kickedUser == Context.User)
            {
                embed = new EmbedBuilder
                {
                    Title = "You can't kick that user"
                }.Build();
            }
            else
            {
                embed = ViolationManager.NewViolation(kickedUser, reason, Context, 2);

                if (embed.Title == "Kicked")
                {
                    await kickedUser.SendMessageAsync(embed: embed);
                    await kickedUser.KickAsync(reason);
                }
            }

            await ReplyAsync(embed: embed);
        }

        /// <summary>
        /// Warn a user
        /// </summary>
        /// <param name="warnedUser">User to warn</param>
        /// <param name="reason" default="No reason specified">Reason for the warn</param>
        /// <returns></returns>
        [Command("warn")]
        public async Task Warn(SocketGuildUser warnedUser, [Remainder] string reason = "No reason specified.")
        {
            Embed embed;
            if (warnedUser == Context.User)
            {
                embed = new EmbedBuilder
                {
                    Title = "You can't warn that user"
                }.Build();
            }
            else if (warnedUser == null)
            {
                embed = new EmbedBuilder
                    {
                        Title = "User not found"
                    }
                    .Build();
            }
            else
            {
                embed = ViolationManager.NewViolation(warnedUser, reason, Context);

                if (embed.Title == "Warned")
                {
                    await warnedUser.SendMessageAsync(embed: embed);
                }
            }

            await ReplyAsync(embed: embed);
        }

        /// <summary>
        /// Ban a user
        /// </summary>
        /// <param name="bannedUser">user to ban</param>
        /// <param name="reason" default="No reason specified"></param>
        /// <returns></returns>
        [Command("ban")]
        public async Task Ban(SocketGuildUser bannedUser, [Remainder] string reason = "No reason specified.")
        {
            Embed embed;
            int prune = 0;
            if (bannedUser == null)
            {
                embed = new EmbedBuilder
                    {
                        Title = "User Not Found"
                    }
                    .Build();
            }
            else if (bannedUser == Context.User)
            {
                embed = new EmbedBuilder
                    {
                        Title = "You can't ban that user"
                    }
                    .Build();
            }
            else
            {
                embed = ViolationManager.NewViolation(bannedUser, reason, Context, 1);

                if (embed.Title == "Banned")
                {
                    await bannedUser.SendMessageAsync(embed: embed);
                    await bannedUser.BanAsync(prune, reason);
                }
            }

            await ReplyAsync(embed: embed);
        }

        /// <summary>
        /// unban a user
        /// </summary>
        /// <param name="bannedUserId">the user to unban</param>
        /// <returns></returns>
        [Command("unban")]
        public async Task Unban(ulong bannedUserId)
        {
            Embed embed = new EmbedBuilder
                {
                    Title = "User Unbanned",
                    Color = Color.Red
                }
                .AddField("User:", "<@!" + bannedUserId + ">", true)
                .AddField("Date", DateTime.Now, true)
                .AddField("Moderator:", Context.User.Mention)
                .WithCurrentTimestamp()
                .WithFooter("UserID: " + bannedUserId)
                .Build();

            await Context.Guild.RemoveBanAsync(bannedUserId);
            await ReplyAsync(embed: embed);
        }

        /// <summary>
        /// Mute a user
        /// </summary>
        /// <param name="mutedUser">User to mute</param>
        /// <param name="reason" default="No reason specified"> The reason for the mute</param>
        /// <returns></returns>
        [Command("mute")]
        public async Task Mute(SocketGuildUser mutedUser, [Remainder] string reason = "No reason specified.")
        {
            Embed embed;
            if (mutedUser == Context.User)
            {
                embed = new EmbedBuilder
                {
                    Title = "You can't mute that user."
                }.Build();
            }
            else
            {
                embed = ViolationManager.NewViolation(mutedUser, reason, Context, 3);

                if (embed.Title == "Muted")
                {
                    await mutedUser.SendMessageAsync(embed: embed);
                    await mutedUser.AddRoleAsync(Context.Guild.GetRole(748884435260276816));
                }
            }

            await ReplyAsync(embed: embed);
        }

        /// <summary>
        /// Unmute a user
        /// </summary>
        /// <param name="unmutedUser">user to unmute</param>
        /// <param name="reason" default="No reason specified"> reason for the unmute</param>
        /// <returns></returns>
        [Command("unmute")]
        public async Task Unmute(SocketGuildUser unmutedUser, [Remainder] string reason = "No reason specified.")
        {
            Embed embed;
            if (unmutedUser == Context.User)
            {
                embed = new EmbedBuilder
                {
                    Title = "You can't unmute that user."
                }.Build();
            }
            else
            {
                embed = ViolationManager.NewViolation(unmutedUser, reason, Context, 4);

                if (embed.Title == "Unmuted")
                {
                    await unmutedUser.SendMessageAsync(embed: embed);
                    await unmutedUser.RemoveRoleAsync(Context.Guild.GetRole(748884435260276816));
                }
            }

            await ReplyAsync(embed: embed);
        }

        /// <summary>
        /// Returns
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        [Command("snowflake")]
        public async Task Test(SocketGuildUser user)
        {
            Embed embed = new EmbedBuilder
                {
                    Title = "Snowflake for user " + user.Username
                }.AddField("snowflake", user.Id)
                .Build();

            await ReplyAsync(embed: embed);
        }
    }
}