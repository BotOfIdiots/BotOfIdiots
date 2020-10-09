using System;
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

            try
            {
                if (userAccount == null)
                {
                    throw new NullReferenceException();
                }

                embed = new EmbedBuilder {}
                    .AddField("User", userAccount.Mention)
                    .WithThumbnailUrl(userAccount.GetAvatarUrl())
                    .AddField("Created At", userAccount.CreatedAt, true)
                    .AddField("Joined At", userAccount.JoinedAt, true)
// TODO:                   .AddField("Roles", userAccount)
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
        //TODO: Userinfo of own account without mentioning it
        /*
        public async Task Userinfo()
        {
            IGuildUser useraccount =  
            try
            {
                var user = message.Author;
                embed = new EmbedBuilder 
                    {
                        Title = message.Author.Mention
                    }
                    .AddField("Created At", user.CreatedAt, true)
                    .AddField("Joined At", userAccount.JoinedAt, true)
                    .AddField("Roles", userAccount.RoleIds.ToString())
                    .WithCurrentTimestamp()
                    .Build();  
            }
            catch (Exception)
            {
                 embed = new EmbedBuilder
                    {
                        Title = "User Not Found"
                    }
                    .WithCurrentTimestamp()
                    .Build();
            }

            await ReplyAsync(embed: embed);
        } 
        */

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
                string reason = "test";
                embed = ViolationManager.NewViolation(bannedUser, reason, Context, 1);
                await bannedUser.SendMessageAsync(embed: embed);
                await bannedUser.BanAsync(prune, reason);
              
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
    }
}
