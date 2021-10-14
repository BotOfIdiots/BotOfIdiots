using System;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using DiscordBot.Database;
using MySql.Data.MySqlClient;

namespace DiscordBot.Modules.Commands
{
    [RequireUserPermission(GuildPermission.ManageChannels)]
    [RequireBotPermission(GuildPermission.ManageChannels)]
    [Group("privatechannels")]
    public class PrivateChannels : ModuleBase<SocketCommandContext>
    {
        [Command("list")]
        public async Task ListPrivateChannels()
        {
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
        

    }
}