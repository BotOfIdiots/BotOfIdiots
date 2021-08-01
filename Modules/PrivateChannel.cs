using System;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Rest;
using Discord.WebSocket;
using DiscordBot.Models;
using DiscordBot.Models.Embeds;

namespace DiscordBot.Modules
{
    public static class PrivateChannel
    {
        private static readonly ulong ChannelCreateId =
            Convert.ToUInt64(DiscordBot.Config.GetSection("PrivateChannels")["CreateChannelId"]);

        private static readonly ulong CategoryId = Convert.ToUInt64(
            DiscordBot.Config.GetSection("PrivateChannels")["CategoryId"]);

        public static async Task CreateChannelHandler(SocketVoiceState stateAfter, SocketUser user)
        {
            if (stateAfter.VoiceChannel.Id == ChannelCreateId)
            {
                OverwritePermissions permissions = new OverwritePermissions(manageChannel: PermValue.Allow);
                
                

                RestVoiceChannel createdChannel = CreateChannel(user);
                await createdChannel.AddPermissionOverwriteAsync(user, permissions);
                MoveUserToCreatedChannel(user, createdChannel);
            }

            await Task.CompletedTask;
        }

        private static RestVoiceChannel CreateChannel(SocketUser user)
        {
            return DiscordBot.Client.GetGuild(DiscordBot.GuildId)
                .CreateVoiceChannelAsync(user.Username + "'s Channel", channelProperties =>
                {
                    channelProperties.CategoryId = CategoryId;
                    channelProperties.UserLimit = 4;
                }).GetAwaiter().GetResult();
        }

        private static void MoveUserToCreatedChannel(SocketUser user, RestVoiceChannel createdChannel)
        {
            IGuildUser guildUser = DiscordBot.Client.GetGuild(DiscordBot.GuildId).GetUser(user.Id);

            guildUser.ModifyAsync(properties => { properties.Channel = createdChannel; });
        }

        public static async Task DestroyChannelHandler(SocketVoiceState stateBefore)
        {
            if (stateBefore.VoiceChannel.Id != ChannelCreateId && stateBefore.VoiceChannel.CategoryId == CategoryId
                                                               && stateBefore.VoiceChannel.Users.Count == 0)
            {
                await stateBefore.VoiceChannel.DeleteAsync();
            }
        }
    }
}