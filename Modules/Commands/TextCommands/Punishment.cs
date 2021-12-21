using System;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using DiscordBot.Class;
using DiscordBot.Database;

namespace DiscordBot.Modules.Commands.TextCommands
{
    /// <summary>
    /// This class contains al the command to punish Guild members
    /// </summary>
    [RequireBotPermission(GuildPermission.KickMembers, ErrorMessage = "The bot is missing the KickMembers permissions")]
    [RequireBotPermission(GuildPermission.BanMembers, ErrorMessage = "The bot is missing BanMembers permissions")]
    [RequireBotPermission(GuildPermission.ManageRoles, ErrorMessage = "The bot is missing the ManageRoles permissions")]
    public class Punishment : ModuleBase<ShardedCommandContext>
    {
        public DatabaseService DatabaseService { set; get; }

        #region Warn Related Commands

        /// <summary>
        /// Warn a user
        /// </summary>
        /// <param name="warnedUser">User to warn</param>
        /// <param name="reason" default="No reason specified">Reason for the warn</param>
        /// <returns></returns>
        [RequireUserPermission(GuildPermission.KickMembers, ErrorMessage = "You don't have permission to warn members")]
        [Command("warn")]
        // [Discord.Commands.Summary("$warn <user/snowflake> <reason> - Warn a user")]
        public async Task Warn(SocketGuildUser warnedUser, [Remainder] string reason)
        {
            try
            {
                await ReplyAsync(embed: HandlePunishment.Warn(Context.Client, DatabaseService,
                    (SocketGuildUser)Context.User, warnedUser, reason));
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
        [Summary("$mute <user/snowflake> <reason> - Mute a user")]
        public async Task Mute(SocketGuildUser mutedUser, [Remainder] string reason)
        {
            try
            {
                await ReplyAsync(embed: HandlePunishment.Mute(Context.Client, DatabaseService,
                    (SocketGuildUser)Context.User, mutedUser, reason));
            }
            catch (Exception e)
            {
                await EventHandlers.LogException(e, Context.Guild);
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
                await ReplyAsync(embed: HandlePunishment.Unmute(Context.Client, DatabaseService,
                    (SocketGuildUser)Context.User, unmutedUser, reason));
            }
            catch (Exception e)
            {
                await EventHandlers.LogException(e, Context.Guild);
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
        [Summary("$kick <user/snowflake> <reason> - Kick a user")]
        public async Task Kick(SocketGuildUser kickedUser, [Remainder] string reason)
        {
            try
            {
                await ReplyAsync(embed: HandlePunishment.Kick(Context.Client, DatabaseService,
                    (SocketGuildUser)Context.User, kickedUser, reason));
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
        /// <param name="reason" default="No reason specified"></param>
        /// <returns></returns>
        [RequireUserPermission(GuildPermission.BanMembers, ErrorMessage = "You don't have permission to ban members")]
        [Command("ban")]
        [Summary("$ban <user/snowflake> <reason> - Ban a user")]
        public async Task Ban(SocketGuildUser bannedUser, [Remainder] string reason)
        {
            try
            {
                await ReplyAsync(embed: HandlePunishment.Ban(Context.Client, DatabaseService,
                    (SocketGuildUser)Context.User, bannedUser, reason));
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
        /// <param name="reason"></param>
        /// <returns></returns>
        [RequireUserPermission(GuildPermission.BanMembers, ErrorMessage = "You don't have permission to unban members")]
        [Command("unban")]
        [Summary("$unban <user/snowflake> {reason} - Unban a banned user")]
        public async Task Unban(ulong bannedUserId, [Remainder] string reason = "No reason specified.")
        {
            try
            {
                await ReplyAsync(embed: HandlePunishment.Unban(Context.Client, DatabaseService,
                    (SocketGuildUser)Context.User, bannedUserId, reason, Context.Guild));
            }
            catch (Exception e)
            {
                await EventHandlers.LogException(e, Context.Guild);
            }
        }

        #endregion
    }
}