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
    }
}
