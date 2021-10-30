using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.Net;
using Discord.WebSocket;
using DiscordBot.Database;

namespace DiscordBot.Modules.Commands
{
    /// <summary>
    /// This class contains al the command to punish Guild members
    /// </summary>
    [RequireBotPermission(GuildPermission.KickMembers, ErrorMessage = "The bot is missing the KickMembers permissions")]
    [RequireBotPermission(GuildPermission.BanMembers, ErrorMessage = "The bot is missing BanMembers permissions")]
    [RequireBotPermission(GuildPermission.ManageRoles, ErrorMessage = "The bot is missing the ManageRoles permissions")]
    public class Punishment : ModuleBase<SocketCommandContext>
    {
       #region Warn Related Commands
        /// <summary>
        /// Warn a user
        /// </summary>
        /// <param name="warnedUser">User to warn</param>
        /// <param name="reason" default="No reason specified">Reason for the warn</param>
        /// <returns></returns>
        [RequireUserPermission(GuildPermission.KickMembers, ErrorMessage = "You don't have permission to warn members")]
        [Command("warn")]
        [Summary("$warn <user/snowflake> {reason} - Warn a user")]
        public async Task Warn(SocketGuildUser warnedUser, [Remainder] string reason = "No reason specified.")
        {
            try
            {
                Embed embed;
                if (warnedUser == Context.User)
                {
                    embed = new EmbedBuilder
                    {
                        Title = "You can't warn that user"
                    }.Build();
                }
                else
                {
                    embed = ViolationManager.NewViolation(warnedUser, reason, Context);

                    await Functions.SendMessageEmbedToUser(warnedUser, embed, Context);
                    await EventHandlers.LogViolation(embed, Context.Guild);
                }

                await ReplyAsync(embed: embed);
            }
            catch (Exception e)
            {
                await EventHandlers.LogException(e, Context.Guild);
            }
        }
        #endregion

        #region Mute Related Commands
        /// <summary>
        /// Mute a user
        /// </summary>
        /// <param name="mutedUser">User to mute</param>
        /// <param name="reason" default="No reason specified"> The reason for the mute</param>
        /// <returns></returns>
        [RequireUserPermission(GuildPermission.KickMembers, ErrorMessage = "You don't have permission to mute members")]
        [Command("mute")]
        [Summary("$mute <user/snowflake> {reason} - Mute a user")]
        public async Task Mute(SocketGuildUser mutedUser, [Remainder] string reason = "No reason specified.")
        {
            Embed embed;

            SocketRole mutedRole = DbOperations.GetMutedRole(mutedUser.Guild);
            
            if (mutedRole == null)
            {
                embed = new EmbedBuilder
                {
                    Title = "Muted Role not defined"
                }.Build();
                await ReplyAsync(embed: embed);
            }
            else
            {
                try
                {
                    if (mutedUser == Context.User)
                    {
                        embed = new EmbedBuilder
                        {
                            Title = "You can't mute that user."
                        }.Build();
                    }

                    else if (mutedUser.Roles.Contains(mutedRole)
                    )
                    {
                        embed = new EmbedBuilder
                        {
                            Title = "User is already muted"
                        }.Build();
                    }

                    else
                    {
                        embed = ViolationManager.NewViolation(mutedUser, reason, Context, 3);

                        await Functions.SendMessageEmbedToUser(mutedUser, embed, Context);
                        await mutedUser.AddRoleAsync(mutedRole);
                        await EventHandlers.LogViolation(embed, Context.Guild);
                    }

                    await ReplyAsync(embed: embed);
                }
                catch (Exception e)
                {
                    await EventHandlers.LogException(e, Context.Guild);
                }
            }
        }

        /// <summary>
        /// Unmute a user
        /// </summary>
        /// <param name="unmutedUser">user to unmute</param>
        /// <param name="reason" default="No reason specified"> reason for the unmute</param>
        /// <returns></returns>
        [RequireUserPermission(GuildPermission.KickMembers, ErrorMessage =
            "You don't have permission to unmute members")]
        [Command("unmute")]
        [Summary("$unmute <user/snowflake> {reason} - Unmute a muted user")]
        public async Task Unmute(SocketGuildUser unmutedUser, [Remainder] string reason = "No reason specified.")
        {
            Embed embed;
            
            SocketRole mutedRole = DbOperations.GetMutedRole(unmutedUser.Guild);
            
            if (DiscordBot.Config["MutedRole"] == null || DiscordBot.Config["MutedRole"] == "")
            {
                embed = new EmbedBuilder
                {
                    Title = "Muted Role not defined"
                }.Build();
                await ReplyAsync(embed: embed);
            }
            else
            {
                try
                {

                    if (unmutedUser == Context.User)
                    {
                        embed = new EmbedBuilder
                        {
                            Title = "You can't unmute that user."
                        }.Build();
                    }
                    else if (!unmutedUser.Roles.Contains(mutedRole))
                    {
                        embed = new EmbedBuilder
                        {
                            Title = "User was not muted"
                        }.Build();
                    }
                    else
                    {
                        embed = ViolationManager.NewViolation(unmutedUser, reason, Context, 4);

                        await Functions.SendMessageEmbedToUser(unmutedUser, embed, Context);
                        await EventHandlers.LogViolation(embed, Context.Guild);
                        await unmutedUser.RemoveRoleAsync(mutedRole);
                    }

                    await ReplyAsync(embed: embed);
                }
                catch (Exception e)
                {
                    await EventHandlers.LogException(e, Context.Guild);
                }
            }
        }
        #endregion

        #region Kick Related Commands
        /// <summary>
        /// Kick a user
        /// </summary>
        /// <param name="kickedUser">User to kick</param>
        /// <param name="reason" default="No reason specified">Reason for the kick</param>
        /// <returns></returns>
        [RequireUserPermission(GuildPermission.KickMembers, ErrorMessage = "You don't have permission to kick members")]
        [Command("kick")]
        [Summary("$kick <user/snowflake> {reason} - Kick a user")]
        public async Task Kick(SocketGuildUser kickedUser, [Remainder] string reason = "No reason specified.")
        {
            try
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

                    await Functions.SendMessageEmbedToUser(kickedUser, embed, Context);
                    await kickedUser.KickAsync(reason);
                    await EventHandlers.LogViolation(embed, Context.Guild);
                }

                await ReplyAsync(embed: embed);
            }
            catch (Exception e)
            {
                await EventHandlers.LogException(e, Context.Guild);
            }
        }
        #endregion

        #region Ban Related Commands
        /// <summary>
        /// Ban a user
        /// </summary>
        /// <param name="bannedUser">user to ban</param>
        /// <param name="prune" default= "0"> Go this amount of days back in time to delete message from the banned user. Must be between 0-7 days </param>
        /// <param name="reason" default="No reason specified"></param>
        /// <returns></returns>
        [RequireUserPermission(GuildPermission.BanMembers, ErrorMessage = "You don't have permission to ban members")]
        [Command("ban")]
        [Summary("$ban <user/snowflake> {prune} {reason} - Ban a user")]
        public async Task Ban(SocketGuildUser bannedUser, [Remainder] string reason = "No reason specified.")
        {
            try
            {
                Embed embed;
                if (bannedUser == Context.User)
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
                    
                    await Functions.SendMessageEmbedToUser(bannedUser, embed, Context);
                    await bannedUser.BanAsync(1, reason);
                    await EventHandlers.LogViolation(embed, Context.Guild);
                }


                await ReplyAsync(embed: embed);
            }
            catch (Exception e)
            {
                await EventHandlers.LogException(e, Context.Guild);
            }
        }

        /// <summary>
        /// unban a user
        /// </summary>
        /// <param name="bannedUserId">the user to unban</param>
        /// <returns></returns>
        [RequireUserPermission(GuildPermission.BanMembers, ErrorMessage = "You don't have permission to unban members")]
        [Command("unban")]
        [Summary("$unban <user/snowflake> {reason} - Unban a banned user")]
        public async Task Unban(ulong bannedUserId, [Remainder] string reason = "No reason specified")
        {
            try
            {
                Embed embed = new EmbedBuilder
                    {
                        Title = "User Unbanned",
                        Color = Color.Red
                    }
                    .AddField("User:", "<@!" + bannedUserId + ">", true)
                    .AddField("Date", DateTime.Now, true)
                    .AddField("Moderator:", Context.User.Mention)
                    .AddField("Reason", reason)
                    .WithCurrentTimestamp()
                    .WithFooter("UserID: " + bannedUserId)
                    .Build();
                await Context.Guild.RemoveBanAsync(bannedUserId);
                await EventHandlers.LogViolation(embed, Context.Guild);
                await ReplyAsync(embed: embed);
            }
            catch (HttpException e)
            {
                if (e.HttpCode == HttpStatusCode.NotFound)
                {
                    Embed embed = new EmbedBuilder()
                        .WithColor(Color.Red)
                        .WithDescription("Banned User not found")
                        .Build();

                    await ReplyAsync(embed: embed);
                }
                else
                {
                    await EventHandlers.LogException(e, Context.Guild);
                }
            }
            catch (Exception e)
            {
                await EventHandlers.LogException(e, Context.Guild);
            }
        }
        #endregion
    }
}