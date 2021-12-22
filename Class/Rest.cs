using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using DiscordBot.Modules;

namespace DiscordBot.Class
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

        public static SocketGuildUser GetUserFromGuild(ulong user, ulong guild, DiscordShardedClient client)
        {
            SocketGuild socketGuild = client.GetGuild(guild);
            SocketGuildUser guildUser = socketGuild.GetUser(user);
            return guildUser;
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