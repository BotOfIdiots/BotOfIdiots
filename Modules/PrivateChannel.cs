using System;
using System.Collections.Generic;
using System.Net.Mail;
using System.Threading;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.Rest;
using Discord.WebSocket;
using DiscordBot.Database;
using MySql.Data.MySqlClient;

namespace DiscordBot.Modules
{
    [RequireUserPermission(GuildPermission.ManageChannels)]
    [RequireBotPermission(GuildPermission.ManageChannels)]
    [Group("privatechannels")]
    public class PrivateChannel : ModuleBase<ShardedCommandContext>
    {
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
                DiscordBot.DbConnection.CheckConnection();
                using MySqlConnection conn = DiscordBot.DbConnection.SqlConnection;
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
                DiscordBot.DbConnection.CheckConnection();
                using MySqlConnection conn = DiscordBot.DbConnection.SqlConnection;
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
        
        #region Commands
        
        [Command("list")]
        public async Task ListPrivateChannels()
        {
            string query = "SELECT CategoryId, CreateChannelId FROM private_channels_setups WHERE Guild = @Guild ";
            
            #region SQL Parameters

            MySqlParameter guild = new MySqlParameter("@Guild", MySqlDbType.UInt64) { Value = Context.Guild.Id };
            
            #endregion
            
            DiscordBot.DbConnection.CheckConnection();
            using MySqlConnection conn = DiscordBot.DbConnection.SqlConnection;
            MySqlDataReader reader = DbOperations.ExecuteReader(conn, query, guild);

            List<ulong> snowflakes = new List<ulong>(2);

            while (reader.Read())
            {
                for (int i = 0; i < reader.FieldCount; i++)
                {
                    snowflakes[i] = reader.GetUInt64(i);
                }
            }

            await ReplyAsync("Category: <#" + snowflakes[0] + ">. Channel: <#" + snowflakes[2] + ">.");
            
            await Task.CompletedTask;
        }

        [Command("set")]
        public async Task SetPrivateChannels(SocketCategoryChannel categoryChannel, SocketVoiceChannel voiceChannel)
        {
            string query = " INSERT INTO private_channels_setups VALUES (@Guild, @Category, @Channel)";
            
            #region SQL Parameters
            MySqlParameter guild = new MySqlParameter("@Guild", MySqlDbType.UInt64) { Value = categoryChannel.Guild.Id };
            MySqlParameter category = new MySqlParameter("@Category", MySqlDbType.UInt64) { Value = categoryChannel.Id };
            MySqlParameter channel = new MySqlParameter("@Channel", MySqlDbType.UInt64) { Value = voiceChannel.Id };
            #endregion
            
            DiscordBot.DbConnection.CheckConnection();
            using MySqlConnection conn = DiscordBot.DbConnection.SqlConnection;
            DiscordBot.DbConnection.ExecuteNonQuery(query, guild, category, channel);

            await Task.CompletedTask;
        }

        [Command("remove")]
        public async Task RemovePrivateChannels()
        {
            await Task.CompletedTask;
        }
        
        #endregion
    }
}