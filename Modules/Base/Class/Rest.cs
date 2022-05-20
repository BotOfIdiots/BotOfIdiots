using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using DiscordBot.Modules.Event;
using Microsoft.Extensions.DependencyInjection;

namespace DiscordBot.Modules.Base.Class
{
    public static class Rest
    {
        public static async Task SendMessageEmbedToUser(SocketGuildUser user, Embed embed, DiscordShardedClient client, SocketGuild guild)
        {
            try
            {
                await user.SendMessageAsync(embed: embed);
            }
            catch (Exception)
            {
                await EventHandlers.LogException(new Exception("Couldn't send message to user"), guild);
            }
        }

        public static SocketGuildUser GetUserFromGuild(ulong user, ulong guild)
        {
            return DiscordBot.Services.GetRequiredService<DiscordShardedClient>().GetGuild(guild).GetUser(user);
        }
        
        public static String CreateRolesList(IReadOnlyCollection<SocketRole> roleCollection)
        {
            String roles = null;
            foreach (IRole role in roleCollection)
            {
                if (roles == null)
                {
                    roles = role.Mention;
                }
                else
                {
                    roles += role.Mention;
                }
            }
            return roles;
        }
    }
}