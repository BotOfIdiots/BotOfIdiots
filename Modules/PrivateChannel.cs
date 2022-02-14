using System;
using System.Threading.Tasks;
using Discord;
using Discord.Rest;
using Discord.WebSocket;
using DiscordBot.Database;
using Microsoft.Extensions.DependencyInjection;
using MySql.Data.MySqlClient;

namespace DiscordBot.Modules
{
    public class PrivateChannel
    {

        private static bool CheckChannel(SocketVoiceChannel voiceChannel, SocketGuild socketGuild, DatabaseService databaseService)
        {
            string query =
                "SELECT CreateChannelId FROM private_channels_setups WHERE Guild = @Guild AND CreateChannelId = @Channel";

            #region SQL Parameters

            MySqlParameter guild = new MySqlParameter("@Guild", MySqlDbType.UInt64) { Value = socketGuild.Id };

            MySqlParameter channel = new MySqlParameter("@Channel", MySqlDbType.UInt64) { Value = voiceChannel.Id };

            #endregion

            try
            {
                MySqlDataReader reader = DbOperations.ExecuteReader(databaseService, query, guild, channel);

                while (reader.Read())
                {
                    if (reader.GetUInt64("CreateChannelId") == voiceChannel.Id)
                    {
                        reader.Close();
                        return true;
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

            return false;
        }
        
        #region Create Private Channel

        public static async Task CreatePrivateChannelHandler(SocketVoiceState stateAfter, SocketUser user,
            IServiceProvider serviceProvider)
        {
            SocketGuild guild = stateAfter.VoiceChannel.Guild;
            SocketVoiceChannel channel = stateAfter.VoiceChannel;

            if (CheckChannel(channel, guild, serviceProvider.GetRequiredService<DatabaseService>()))
            {
                OverwritePermissions permissions = new OverwritePermissions(manageChannel: PermValue.Allow);
                RestVoiceChannel createdChannel = CreateChannel(user, guild, channel.Category.Id);
                await createdChannel.AddPermissionOverwriteAsync(user, permissions);
                await MoveUserToCreatedChannel(user, createdChannel, guild);
            }

            await Task.CompletedTask;
        }

        private static RestVoiceChannel CreateChannel(SocketUser user, SocketGuild guild, ulong categoryId)
        {
            return guild.CreateVoiceChannelAsync(user.Username + "'s Channel", channelProperties =>
                {
                    channelProperties.CategoryId = categoryId;
                    channelProperties.UserLimit = 4;
                }
            ).GetAwaiter().GetResult();
        }

        private static Task MoveUserToCreatedChannel(SocketUser user, RestVoiceChannel createdChannel, SocketGuild guild)
        {
            IGuildUser guildUser = guild.GetUser(user.Id);

            guildUser.ModifyAsync(properties => { properties.Channel = createdChannel; });
            return Task.CompletedTask;
        }

        #endregion

        #region Remove Private Channel

        public static async Task DestroyPrivateChannelHandler(SocketVoiceState stateBefore,
            IServiceProvider serviceProvider)
        {
            SocketVoiceChannel channel = stateBefore.VoiceChannel;
            SocketGuild guild = stateBefore.VoiceChannel.Guild;

            if (CheckDestroyChannel(channel, guild, serviceProvider.GetRequiredService<DatabaseService>()))
            {
                await stateBefore.VoiceChannel.DeleteAsync();
            }
        }

        private static bool CheckDestroyChannel(SocketVoiceChannel channel, SocketGuild guild,
            DatabaseService databaseService)
        {
            try
            {
                ICategoryChannel category = channel.Category;
                if (CheckCategory(category.Id, guild.Id, databaseService))
                {
                    if (channel.Users.Count == 0 && !CheckChannel(channel, guild, databaseService))
                    {
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }

            return false;
        }

        public static bool CheckCategory(ulong categoryChannel, ulong socketGuild, DatabaseService databaseService)
        {
            string query =
                "SELECT CategoryId FROM private_channels_setups WHERE CategoryId = @Category AND Guild = @Guild";

            #region SQL Parameters

            MySqlParameter guild = new MySqlParameter("@Guild", MySqlDbType.UInt64) { Value = socketGuild };

            MySqlParameter category = new MySqlParameter("@Category", MySqlDbType.UInt64) { Value = categoryChannel };

            #endregion

            try
            {
                using MySqlDataReader reader = DbOperations.ExecuteReader(databaseService, query, category, guild);
                while (reader.Read())
                {
                    if (reader.GetUInt64("CategoryId") == categoryChannel)
                    {
                        reader.Close();
                        return true;
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            return false;
        }

        #endregion
    }
}