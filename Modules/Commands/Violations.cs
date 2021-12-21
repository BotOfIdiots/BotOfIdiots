using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using DiscordBot.Database;
using DiscordBot.Objects;

namespace DiscordBot.Modules.Commands
{
    [RequireBotPermission(GuildPermission.KickMembers, ErrorMessage =
        "The Bot doesn't have the KickMembers permission")]
    [RequireUserPermission(GuildPermission.KickMembers, ErrorMessage = "You don't have permission to use this command")]
    [Group("violation")]
    [Summary("Everything to do with violations")]
    public class Violations : ModuleBase<ShardedCommandContext>
    {
        #region Services

        public DatabaseService DatabaseService { get; set; }

        #endregion

        /// <summary>
        /// Get a specific violation
        /// </summary>
        /// <param name="violationId"></param>
        /// <returns></returns>
        [Command]
        [Summary("$violation <violationID> - Returns the violation")]
        public async Task Violation(int violationId)
        {
            try
            {
                Embed embed = ViolationManager.GetViolation(violationId, Context, DatabaseService);

                await ReplyAsync(embed: embed);
            }
            catch (Exception e)
            {
                await EventHandlers.LogException(e, Context.Guild);
            }
        }

        /// <summary>
        /// Get the violations of a user
        /// </summary>
        /// <param name="guildUser"></param>
        /// <returns></returns>
        [Command("list")]
        [Summary("$violation list <user/snowflake> - Returns a list of violations")]
        public async Task List(SocketGuildUser guildUser)
        {
            try
            {
                List<Violation> violations =
                    ViolationManager.GetViolations(guildUser.Id, Context.Guild.Id, DatabaseService, Context.Client);

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

                    embedBuilder.AddField("Violation", violation.ViolationId + " - " + violationType);
                }

                Embed embed = embedBuilder.Build();

                await ReplyAsync(embed: embed);
            }
            catch (Exception e)
            {
                await EventHandlers.LogException(e, Context.Guild);
            }
        }

        [RequireUserPermission(GuildPermission.Administrator, ErrorMessage =
            "You don't have persmision to use this command")]
        [Command("remove")]
        [Summary("$violation remove <violationID> - Removes a violation")]
        public async Task Remove(int violationId, [Remainder] String reason = "No reason specified")
        {
            try
            {
                Violation violation =
                    Objects.Violation.Select(Context.Guild.Id, violationId, DatabaseService, Context.Client);
                Embed embed = new EmbedBuilder
                    {
                        Title = "Violation Removed",
                        Color = Color.Orange
                    }
                    .WithAuthor(Context.Client.CurrentUser)
                    .AddField("User", "<@!" + violation.User + ">", true)
                    .AddField("Date", DateTime.Now, true)
                    .AddField("Moderator", Context.User.Mention)
                    .AddField("Reason", reason)
                    .AddField("Original Violation Date", violation.Date)
                    .AddField("Original Violation Reason", violation.Reason)
                    .WithFooter("UserID: " + violation.User)
                    .WithTimestamp(DateTime.Now)
                    .Build();

                violation.Remove();
                await ReplyAsync(embed: embed);
            }
            catch (Exception e)
            {
                await EventHandlers.LogException(e, Context.Guild);
            }
        }
    }
}