using System;
using System.Threading.Tasks;
using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using DiscordBot.Class;
using DiscordBot.Database;

namespace DiscordBot.Modules.Commands.TextCommands
{
    /// <summary>
    /// This class contains al the command for punishing Guild members
    /// </summary>
    [RequireBotPermission(GuildPermission.KickMembers)]
    [RequireBotPermission(GuildPermission.BanMembers)]
    [RequireBotPermission(GuildPermission.ModerateMembers)]
    public class Punishment : InteractionModuleBase<ShardedInteractionContext>
    {
        /// <summary>
        /// The database service to be use when performing commands
        /// </summary>
        public DatabaseService DatabaseService { set; get; }

        /// <summary>
        /// Command for warning a user
        /// </summary>
        /// <param name="user">User to warn</param>
        /// <param name="reason" default="No reason specified">Reason for the warn</param>
        /// <returns></returns>
        [RequireUserPermission(GuildPermission.ModerateMembers)]
        [SlashCommand("warn", "Warns the specified user")]
        public async Task Warn(SocketGuildUser user, string reason)
        {
            try
            {
                await RespondAsync(embed: HandlePunishment.Warn(Context.Client, DatabaseService,
                    (SocketGuildUser)Context.User, user, reason));
            }
            catch (Exception e)
            {
                // await EventHandlers.LogException(e, Context.Guild);
            }
        }
        
        /// <summary>
        /// Contains all commands related to timing out users
        /// </summary>
        [RequireUserPermission(GuildPermission.ModerateMembers)]
        [Group("time-out", "Mute the specified user")]
        class TimeOut : Punishment
        {
            /// <summary>
            /// Command for timing out a user
            /// </summary>
            /// <param name="user">User to time out</param>
            /// <param name="reason" default="No reason specified"> The reason for the time out</param>
            /// <returns></returns>
            // [SlashCommand("set", "Time-out the specified member")]
            public async Task Set(SocketGuildUser user, string length, string reason)
            {
                try
                {
                    await RespondAsync(embed: HandlePunishment.Mute((SocketGuildUser)Context.User, user, length, reason));
                }
                catch (Exception e)
                {
                    // await EventHandlers.LogException(e, Context.Guild);
                }
            }

            /// <summary>
            /// Command for removing a time out from a user
            /// </summary>
            /// <param name="user"> User to remove time out from </param>
            /// <param name="reason" default="No reason specified"> reason for the removal of the time out </param>
            /// <returns></returns>
            [SlashCommand("remove", "Remove the time-out from specified User")]
            public async Task Remove(SocketGuildUser user, string reason = "No reason specified.")
            {
                try
                {
                    await RespondAsync(embed: HandlePunishment.Unmute(Context.Client, DatabaseService,
                        (SocketGuildUser)Context.User, user, reason));
                }
                catch (Exception e)
                {
                    // await EventHandlers.LogException(e, Context.Guild);
                }
            }
        }

        /// <summary>
        /// Command for kicking a user
        /// </summary>
        /// <param name="user">User to kick</param>
        /// <param name="reason" default="No reason specified">Reason for the kick</param>
        /// <returns></returns>
        [RequireUserPermission(GuildPermission.KickMembers)]
        [SlashCommand("kick", "Kick the specified user from the guild")]
        public async Task Kick(SocketGuildUser user, string reason)
        {
            try
            {
                await RespondAsync(embed: HandlePunishment.Kick(Context.Client, DatabaseService,
                    (SocketGuildUser)Context.User, user, reason));
            }
            catch (Exception e)
            {
                await EventHandlers.LogException(e, Context.Guild);
            }
        }
        
        /// <summary>
        /// Command for banning a user
        /// </summary>
        /// <param name="user">user to ban</param>
        /// <param name="reason" default="No reason specified"></param>
        /// <returns></returns>
        [RequireUserPermission(GuildPermission.BanMembers)]
        [SlashCommand("ban", "Ban the specified user from the guild")]
        public async Task Ban(SocketGuildUser user, string reason)
        {
            try
            {
                await RespondAsync(embed: HandlePunishment.Ban(Context.Client, DatabaseService,
                    (SocketGuildUser)Context.User, user, reason));
            }
            catch (Exception e)
            {
                await EventHandlers.LogException(e, Context.Guild);
            }
        }
        
        /// <summary>
        /// Command for unbanning a user
        /// </summary>
        /// <param name="userId">the user to unban</param>
        /// <param name="reason"></param>
        /// <returns></returns>
        [RequireUserPermission(GuildPermission.BanMembers)]
        [SlashCommand("unban", "Unban the specified user")]
        public async Task Unban(ulong userId, string reason = "No reason specified.")
        {
            try
            {
                await RespondAsync(embed: HandlePunishment.Unban(Context.Client, DatabaseService,
                    (SocketGuildUser)Context.User, userId, reason, Context.Guild));
            }
            catch (Exception e)
            {
                await EventHandlers.LogException(e, Context.Guild);
            }
        }
    }
}