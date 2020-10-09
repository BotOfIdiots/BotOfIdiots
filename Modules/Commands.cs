using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using DiscordBot.Modules;

namespace DiscordBot.Modules
{
    public class Commands : ModuleBase<SocketCommandContext>
    {
        [Command("ping")]
        public async Task Ping()
        {
            await ReplyAsync("Pong");
        }

        [Command("version")]
        // Return the current version of the bot
        public async Task Version()
        {
            Embed embed = new EmbedBuilder
            {
                Title = "Version: " + Program.Version(),
            }
                .WithAuthor(Context.Client.CurrentUser)
                .WithFooter(Program.Version())
                .WithCurrentTimestamp()
                .Build();
            
            await ReplyAsync(embed: embed);
        }

        [Command("userinfo")]
        // Get the account information of a user
        public async Task Userinfo(IGuildUser userAccount = null)
        {
            Embed embed;
            string userRoles = null;
            IReadOnlyCollection<ulong> userRoleIDs;

            try
            {
                if (userAccount == null)
                {
                    userAccount = Context.Guild.GetUser(Context.User.Id);
                }

                userRoleIDs = userAccount.RoleIds;

                foreach (ulong roleID in userRoleIDs)
                {
                    if (userRoles == null)
                    {
                        userRoles = Context.Guild.GetRole(roleID).Mention;
                    }
                    else
                    {
                        userRoles = userRoles + ", " + Context.Guild.GetRole(roleID).Mention;
                    }
                }

                embed = new EmbedBuilder {}
                    .AddField("User", userAccount.Mention)
                    .WithThumbnailUrl(userAccount.GetAvatarUrl())
                    .AddField("Created At", userAccount.CreatedAt.ToString("MM-dd-yy HH:mm:ss"), true)
                    .AddField("Joined At", userAccount.JoinedAt?.ToString("MM-dd-yy HH:mm:ss"), true)
                    .AddField("Roles", userRoles)
                    .WithAuthor(userAccount)
                    .WithFooter("UserID: " + userAccount.Id)
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
            catch(Exception e)
            {
                Console.WriteLine(e);   
            }
        }
    }
}
