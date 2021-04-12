using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.Net;
using Discord.WebSocket;
using DiscordBot.Models;

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
        public static readonly IRole MutedRole = DiscordBot.Client.GetGuild(DiscordBot.GuildId)
            .GetRole(Convert.ToUInt64(DiscordBot.Config["MutedRole"]));

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
                if (warnedUser == Context.User)
                {
                    await ReplyAsync(embed: Functions.CommandError("You can't warn that user"));
                }

                else
                {
                    await ReplyAsync(embed: await ViolationManager.NewViolation(warnedUser, reason, Context, 0));
                }
            }
            catch (Exception e)
            {
                await EventHandlers.LogException(e);
            }
        }

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
            try
            {
                if (mutedUser == Context.User)
                {
                    await ReplyAsync(embed: Functions.CommandError("You can't mute that user."));
                }

                else if (mutedUser.Roles.Contains(MutedRole)
                )
                {
                    await ReplyAsync(embed: Functions.CommandError("User is already muted"));
                }

                else
                {
                    await ReplyAsync(embed: await ViolationManager.NewViolation(mutedUser, reason, Context, ViolationTypes.Muted));
                }
            }
            catch (NullReferenceException e)
            {
                if (DiscordBot.Config["MutedRole"] == null || DiscordBot.Config["MutedRole"] == "")
                {
                    await ReplyAsync(embed: Functions.CommandError("Muted Role not defined"));
                }
                
                else
                {
                    await EventHandlers.LogException(e);
                }
            }
            catch (Exception e)
            {
                await EventHandlers.LogException(e);
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
            try
            {
                if (unmutedUser == Context.User)
                {
                    await ReplyAsync(embed: Functions.CommandError("You can't unmute that user."));
                }
                
                else if (!unmutedUser.Roles.Contains(MutedRole))
                {
                    await ReplyAsync(embed: Functions.CommandError("User was not muted"));
                }
                
                else
                {
                    await ReplyAsync(embed: await ViolationManager.NewViolation(unmutedUser, reason, Context, ViolationTypes.UnMuted));
                }
            }
            catch (NullReferenceException e)
            {
                if (DiscordBot.Config["MutedRole"] == null || DiscordBot.Config["MutedRole"] == "")
                {
                    await ReplyAsync(embed: Functions.CommandError("Muted Role not defined"));
                }
                
                else
                {
                    await EventHandlers.LogException(e);
                }
            }
            catch (Exception e)
            {
                await EventHandlers.LogException(e);
            }
        }

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
                if (kickedUser == Context.User)
                {
                    await ReplyAsync(embed: Functions.CommandError("You can't kick that user"));
                }
                
                else
                {
                    await ReplyAsync(embed: 
                        await ViolationManager.NewViolation(kickedUser, reason, Context, ViolationTypes.Kicked));
                }
            }
            catch (Exception e)
            {
                await EventHandlers.LogException(e);
            }
        }

        /// <summary>
        /// Ban a user
        /// </summary>
        /// <param name="bannedUser">user to ban</param>
        /// <param name="reason" default="No reason specified">The reason for the ban</param>
        /// <returns></returns>
        [RequireUserPermission(GuildPermission.BanMembers, ErrorMessage = "You don't have permission to ban members")]
        [Command("ban")]
        [Summary("$ban <user/snowflake> {reason} - Ban a user")]
        public async Task Ban(SocketGuildUser bannedUser, [Remainder] string reason = "No reason specified.")
        {
            try
            {
                if (bannedUser == Context.User)
                {
                    await ReplyAsync(embed: Functions.CommandError("You can't ban that user"));
                }
                
                else
                {
                    await ReplyAsync(embed: 
                        await ViolationManager.NewViolation(bannedUser, reason, Context, ViolationTypes.Banned));
                }
            }
            catch (Exception e)
            {
                await EventHandlers.LogException(e);
            }
        }

        /// <summary>
        /// Ban a user
        /// </summary>
        /// <param name="bannedUserId"> Id of the user to ban</param>
        /// <param name="reason" default="No reason specified">The reason for the ban</param>
        /// <returns></returns>
        [RequireUserPermission(GuildPermission.BanMembers, ErrorMessage = "You don't have permission to ban members")]
        [Command("ban")]
        [Summary("$ban <user/snowflake> {reason} - Ban a user")]
        public async Task Ban(ulong bannedUserId, [Remainder] string reason = "No reason specified.")
        {
            try
            {
                if (bannedUserId == Context.User.Id)
                {
                    await ReplyAsync(embed: Functions.CommandError("You can't ban that user"));
                }

                else
                {
                    await Context.Guild.AddBanAsync(bannedUserId, 1, reason);
                    await ReplyAsync(embed: 
                        ViolationManager.CreateViolationRecord(bannedUserId, reason, Context, ViolationTypes.Banned)
                        );
                }
            }
            catch (Exception e)
            {
                await EventHandlers.LogException(e);
            }
        }

        /// <summary>
        /// unban a user
        /// </summary>
        /// <param name="bannedUserId">the user to unban</param>
        /// <param name="reason"></param>
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
                    await EventHandlers.LogException(e);
                }
            }
            catch (Exception e)
            {
                await EventHandlers.LogException(e);
            }
        }
    }
}