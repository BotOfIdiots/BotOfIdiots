using System;
using System.Threading;
using System.Threading.Tasks;
using Discord;
using Discord.Rest;
using Discord.WebSocket;
using DiscordBot.Database;
using DiscordBot.DiscordApi;
using MySql.Data.MySqlClient;

namespace DiscordBot.Modules
{
    public static class PrivateChannel
    {
        #region Methods

        #region Create Private Channel

        public static async Task CreatePrivateChannelHandler(SocketVoiceState stateAfter, SocketUser user)
        {
            SocketGuild guild = stateAfter.VoiceChannel.Guild;
            SocketVoiceChannel channel = stateAfter.VoiceChannel;

            if (CheckChannel(channel, guild))
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

        public static async Task DestroyPrivateChannelHandler(SocketVoiceState stateBefore)
        {
            SocketVoiceChannel channel = stateBefore.VoiceChannel;
            SocketGuild guild = stateBefore.VoiceChannel.Guild;

            if (CheckDestroyChannel(channel, guild))
            {
                await stateBefore.VoiceChannel.DeleteAsync();
            }
        }

        private static bool CheckDestroyChannel(SocketVoiceChannel channel, SocketGuild guild)
        {
            try
            {
                ICategoryChannel category = channel.Category;
                if (CheckCategory(category.Id, guild.Id))
                {
                    if (channel.Users.Count == 0 && !CheckChannel(channel, guild))
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

        public static bool CheckCategory(ulong categoryChannel, ulong socketGuild)
        {
            string query =
                "SELECT CategoryId FROM private_channels_setups WHERE CategoryId = @Category AND Guild = @Guild";

            #region SQL Parameters

            MySqlParameter guild = new MySqlParameter("@Guild", MySqlDbType.UInt64) { Value = socketGuild };

            MySqlParameter category = new MySqlParameter("@Category", MySqlDbType.UInt64) { Value = categoryChannel };

            #endregion

            try
            {
                Bot.DbConnection.CheckConnection();
                using MySqlConnection conn = Bot.DbConnection.SqlConnection;
                MySqlDataReader reader = DbOperations.ExecuteReader(conn, query, category, guild);

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

        private static bool CheckChannel(SocketVoiceChannel voiceChannel, SocketGuild socketGuild)
        {
            string query =
                "SELECT CreateChannelId FROM private_channels_setups WHERE Guild = @Guild AND CreateChannelId = @Channel";

            #region SQL Parameters

            MySqlParameter guild = new MySqlParameter("@Guild", MySqlDbType.UInt64) { Value = socketGuild.Id };

            MySqlParameter channel = new MySqlParameter("@Channel", MySqlDbType.UInt64) { Value = voiceChannel.Id };

            #endregion

            try
            {
                Bot.DbConnection.CheckConnection();
                using MySqlConnection conn = Bot.DbConnection.SqlConnection;
                MySqlDataReader reader = DbOperations.ExecuteReader(conn, query, guild, channel);

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

        #endregion
    }
}