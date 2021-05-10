using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using DiscordBot.Models;
using DiscordBot.Models.Embeds;

namespace DiscordBot.Modules.Commands
{
    
    [RequireBotPermission(GuildPermission.KickMembers, ErrorMessage =
        "The Bot doesn't have the KickMembers permission")]
    [RequireUserPermission(GuildPermission.KickMembers, ErrorMessage = "You don't have permission to use this command")]
    [Group("violation")]
    [Summary("Everything to do with violations")]
    public class Violations : ModuleBase<SocketCommandContext>
    {
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
                await ReplyAsync(embed: new ViolationEmbedBuilder(violationId, Context.Client.CurrentUser).Build());
            }
            catch (Exception e)
            {
                await EventHandlers.LogException(e);
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
                List<Violation> violations = ViolationManager.GetViolations(guildUser.Id);

                EmbedBuilder embedBuilder = new EmbedBuilder();
                embedBuilder.WithTitle("Violations")
                    .AddField("User:", guildUser.Mention)
                    .AddField("ViolationCount", violations.Count)
                    .WithCurrentTimestamp()
                    .WithFooter("UserID: " + guildUser.Id);

                foreach (Violation violation in violations)
                    embedBuilder.AddField(
                        violation.Id + " - " +  violation.Type + ":",
                        violation.Reason
                        );
                
                Embed violationList = embedBuilder.Build();

                await ReplyAsync(embed: violationList);
            }
            catch (Exception e)
            {
                await EventHandlers.LogException(e);
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
                Violation violation = Models.Violation.GetRecordById(violationId);
                Embed embed = new EmbedBuilder
                    {
                        Title = "Violation Removed",
                        Color = Color.Orange
                    }
                    .WithAuthor(Context.Client.CurrentUser)
                    .AddField("User", "<@!" + violation.UserId + ">", true)
                    .AddField("Date", DateTime.Now, true)
                    .AddField("Moderator", Context.User.Mention)
                    .AddField("Reason", reason)
                    .AddField("Original Violation Date", violation.Date)
                    .AddField("Original Violation Reason", violation.Reason)
                    .WithFooter("UserID: " + violation.UserId)
                    .WithTimestamp(DateTime.Now)
                    .Build();
                
                Models.Violation.DeleteRecord(violationId);
                await ReplyAsync(embed: embed);
            }
            catch (Exception e)
            {
                await EventHandlers.LogException(e);
            }
        }
    }
}