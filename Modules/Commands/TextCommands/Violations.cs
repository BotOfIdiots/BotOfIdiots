using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using DiscordBot.Database;
using DiscordBot.Objects;

namespace DiscordBot.Modules.Commands.TextCommands
{
    [RequireBotPermission(GuildPermission.KickMembers)]
    [RequireUserPermission(GuildPermission.KickMembers)]
    [Group("violation", "Command for managing violations")]
    public class Violations : InteractionModuleBase<ShardedInteractionContext>
    {
        public DatabaseService DatabaseService { get; set; }
        
        /// <summary>
        /// Get a specific violation
        /// </summary>
        /// <param name="violationId"></param>
        /// <returns></returns>
        [SlashCommand("get", "Returns the specified violation")]
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
        [SlashCommand("list", "Return the list of violations for the specified user")]
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

        /// <summary>
        /// Remove a specific violation
        /// </summary>
        /// <param name="violationId"></param>
        /// <param name="reason"></param>
        [RequireUserPermission(GuildPermission.ManageGuild)]
        [SlashCommand("remove", "Remove the specified violation")]
        public async Task Remove(int violationId, String reason = "No reason specified")
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