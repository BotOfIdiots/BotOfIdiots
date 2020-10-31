using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;

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
                Title = "Version: " + DiscordBot.Version(),
            }
                .WithAuthor(Context.Client.CurrentUser)
                .WithFooter(DiscordBot.Version())
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
                    .AddField("Violation Count:", ViolationManager.ViolationCount(userAccount.Id.ToString()))
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
        [Command("ban")]
        public async Task Ban(IGuildUser bannedUser, params String[] parameters)
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
            else if(bannedUser == Context.User)
            {
                embed = new EmbedBuilder
                {
                  Title  = "You can't ban that user"
                }
                    .Build();
            }
            else
            {
                string reason = parameters[0];
                for (int i = 1; i < parameters.Length; i++)
                {
                    reason += " " + parameters[i];
                }
                
                embed = ViolationManager.NewViolation(bannedUser, reason, Context, "1");

                if (embed.Title == "Banned")
                {
                    await bannedUser.SendMessageAsync(embed: embed);
                    await bannedUser.BanAsync(prune, reason);
                }
            }
            await ReplyAsync(embed: embed);
        }
        
        [Command("warn")]
        public async Task Warn(IGuildUser warnedUser, params String[] parameters)
        {
            Embed embed;
            if (warnedUser == Context.User)
            {
                embed = new EmbedBuilder
                {
                    Title = "You can't warn that user"
                }.Build();
            }
            else if(warnedUser == null)
            {
                embed = new EmbedBuilder
                {
                    Title = "User not found"
                }
                    .Build();
            }
            else
            {
                string reason = parameters[0];
                for (int i = 1; i < parameters.Length; i++)
                {
                    reason += " " + parameters[i];
                }
                
                embed = ViolationManager.NewViolation(warnedUser, reason, Context, "4");
                    
                if (embed.Title == "Warned")
                {
                    await warnedUser.SendMessageAsync(embed: embed);
                }
                
            }
            await ReplyAsync(embed: embed);
        }

        [Command("kick")]
        public async Task Kick(IGuildUser kickedUser, params String[] parameters)
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
                string reason = parameters[0];
                for (int i = 1; i < parameters.Length; i++)
                {
                    reason += " " + parameters[i];
                }
                
                embed = ViolationManager.NewViolation(kickedUser, reason, Context, "2");
                
                if (embed.Title == "Kicked")
                {
                    await kickedUser.SendMessageAsync(embed: embed);
                    await kickedUser.KickAsync(reason);
                }
            }

            await ReplyAsync(embed: embed);
        }
        
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

        [Command("mute")]
        public async Task Mute(SocketGuildUser mutedUser, [Remainder] string reason = "No reason specified.")
        {
            Embed embed;
            if (mutedUser == Context.User)
            {
                embed = new EmbedBuilder
                {
                    Title = "You can't mute that user"
                }.Build();
            }
            else
            {
                embed = ViolationManager.NewViolation(mutedUser, reason, Context, "3");

                if (embed.Title == "Muted")
                {
                    await mutedUser.SendMessageAsync(embed: embed);
                    await mutedUser.AddRoleAsync(Context.Guild.GetRole(748884435260276816));
                }
            }

            await ReplyAsync(embed: embed);
        }

        [Command("test")]
        public async Task Test(IGuildUser test)
        {
            Embed embed = new EmbedBuilder
            {
                Title = test.Id.ToString()
            }.Build();

            await ReplyAsync(embed: embed);
        }
    }
}
