using System;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using MySql.Data.MySqlClient;

namespace DiscordBot.Modules.Commands
{
    [RequireUserPermission(GuildPermission.Administrator, ErrorMessage = "You need the Administrator permission to execute this command")]
    [Group("set")]
    public class Set : ModuleBase<SocketCommandContext>
    {
        #region Set Moderation Roles
        [Command("joinrole")]
        [Summary("$set joinrole <role/snowflake> - Set the join role")]
        public async Task JoinRole(IRole role)
        {
            string query = "UPDATE guild_configurations  SET JoinRole = @Role WHERE Guild = @Guild";

            MySqlParameter guild = new MySqlParameter("@Guild", MySqlDbType.UInt64);
            guild.Value = role.Guild.Id;

            MySqlParameter joinRole = new MySqlParameter("@Role", MySqlDbType.UInt64);
            joinRole.Value = role.Id;

            int succes = DiscordBot.DbConnection.ExecuteNonQuery(query, guild, joinRole);

            if (succes == 1)
            {
                Embed reactionEmbed = new EmbedBuilder
                    {
                        Color = Color.Green
                    }
                    .WithDescription("Join role succesfully set to: " + role.Mention)
                    .Build();

                await ReplyAsync(embed: reactionEmbed);
            }

            if (succes == -1)
            {
                Embed reactionEmbed = new EmbedBuilder
                    {
                        Color = Color.Red
                    }
                    .WithDescription("Could not set join role to: " + role.Mention)
                    .Build();

                await ReplyAsync(embed: reactionEmbed);
            }
        }
        
        [Command("muterole")]
        [Summary("$set muterole <role/snowflake> - Set the mute role")]
        public async Task MuteRole(IRole role)
        {
            string query = "UPDATE guild_configurations  SET MutedRole = @Role WHERE Guild = @Guild";

            MySqlParameter guild = new MySqlParameter("@Guild", MySqlDbType.UInt64);
            guild.Value = role.Guild.Id;

            MySqlParameter joinRole = new MySqlParameter("@Role", MySqlDbType.UInt64);
            joinRole.Value = role.Id;

            int succes = DiscordBot.DbConnection.ExecuteNonQuery(query, guild, joinRole);

            if (succes == 1)
            {
                Embed reactionEmbed = new EmbedBuilder
                    {
                        Color = Color.Green
                    }
                    .WithDescription("Mute role succesfully set to: " + role.Mention)
                    .Build();

                await ReplyAsync(embed: reactionEmbed);
            }

            if (succes == -1)
            {
                Embed reactionEmbed = new EmbedBuilder
                    {
                        Color = Color.Red
                    }
                    .WithDescription("Could not set mute role to: " + role.Mention)
                    .Build();

                await ReplyAsync(embed: reactionEmbed);
            }
        }
        
        [Command("moderationrole")]
        [Summary("$set moderationrole <role/snowflake> - Set the moderation role")]
        public async Task ModerationRole(IRole role)
        {
            string query = "UPDATE guild_configurations  SET ModerationRole= @Role WHERE Guild = @Guild";

            MySqlParameter guild = new MySqlParameter("@Guild", MySqlDbType.UInt64);
            guild.Value = role.Guild.Id;

            MySqlParameter joinRole = new MySqlParameter("@Role", MySqlDbType.UInt64);
            joinRole.Value = role.Id;

            int succes = DiscordBot.DbConnection.ExecuteNonQuery(query, guild, joinRole);

            if (succes == 1)
            {
                Embed reactionEmbed = new EmbedBuilder
                    {
                        Color = Color.Green
                    }
                    .WithDescription("Moderation role succesfully set to: " + role.Mention)
                    .Build();

                await ReplyAsync(embed: reactionEmbed);
            }

            if (succes == -1)
            {
                Embed reactionEmbed = new EmbedBuilder
                    {
                        Color = Color.Red
                    }
                    .WithDescription("Could not set moderation role to: " + role.Mention)
                    .Build();

                await ReplyAsync(embed: reactionEmbed);
            }
        }
        #endregion
        
        #region Setup Logging
        [Command("logging")]
        public async Task Logging(SocketTextChannel logsChannel, SocketTextChannel exceptionsChannel)
        {
            string query = "INSERT INTO log_channels_settings (Guild, Logs, Exceptions) VALUE (@Guild, @Logs, @Exceptions)";

            #region SQL Parameters
            MySqlParameter guild = new MySqlParameter("@Guild", MySqlDbType.UInt64);
            guild.Value = logsChannel.Guild.Id;
            
            MySqlParameter logs = new MySqlParameter("@Logs", MySqlDbType.UInt64);
            logs.Value = logsChannel.Id;
            
            MySqlParameter exceptions = new MySqlParameter("@Exceptions", MySqlDbType.UInt64);
            exceptions.Value = exceptionsChannel.Id;
            #endregion
            
            int succes = DiscordBot.DbConnection.ExecuteNonQuery(query, guild, logs, exceptions);
            
            if (succes == 1)
            {
                Embed reactionEmbed = new EmbedBuilder
                    {
                        Color = Color.Green
                    }
                    .WithDescription("Succesfully enabled logging")
                    .Build();

                await ReplyAsync(embed: reactionEmbed);
            }

            if (succes == -1)
            {
                Embed reactionEmbed = new EmbedBuilder
                    {
                        Color = Color.Red
                    }
                    .WithDescription("Could not enable logging")
                    .Build();

                await ReplyAsync(embed: reactionEmbed);
            }
        }
        
        [Command("logchannel")]
        public async Task LogChannel(string logType, SocketTextChannel logChannel)
        {
            string query = "UPDATE log_channels_settings SET " + logType + " = @logs WHERE Guild = @Guild";

            MySqlParameter guild = new MySqlParameter("@Guild", MySqlDbType.UInt64);
            guild.Value = logChannel.Guild.Id;
            
            MySqlParameter channel = new MySqlParameter("@Logs", MySqlDbType.UInt64);
            channel.Value = logChannel.Id;

            int succes = DiscordBot.DbConnection.ExecuteNonQuery(query, channel, guild);
            
            if (succes == 1)
            {
                Embed reactionEmbed = new EmbedBuilder
                    {
                        Color = Color.Green
                    }
                    .WithDescription("Succesfully set log channel")
                    .Build();

                await ReplyAsync(embed: reactionEmbed);
            }

            if (succes == -1)
            {
                Embed reactionEmbed = new EmbedBuilder
                    {
                        Color = Color.Red
                    }
                    .WithDescription("Could not set log channel")
                    .Build();

                await ReplyAsync(embed: reactionEmbed);
            }

        }
        #endregion

        #region Private Channels

        [Command("privatechannel")]
        public async Task PrivateChannel(ulong categorySnowflake, ulong voiceChannelSnowflake)
        {
            String query = "INSERT INTO private_channels_setups VALUE (@Guild, @Category, @Channel);";

            #region SQL Parameters
            MySqlParameter guild = new MySqlParameter("@Guild", MySqlDbType.UInt64) { Value = Context.Guild.Id };
            MySqlParameter category = new MySqlParameter("@Category", MySqlDbType.UInt64) { Value = categorySnowflake };
            MySqlParameter channel = new MySqlParameter("@Channel", MySqlDbType.UInt64)
                { Value = voiceChannelSnowflake };
            #endregion

            int succes = DiscordBot.DbConnection.ExecuteNonQuery(query, channel, guild, category);

            if (succes == 1)
            {
                Embed reactionEmbed = new EmbedBuilder
                    {
                        Color = Color.Green
                    }
                    .WithDescription("Succesfully enabled Private Channels")
                    .Build();

                await ReplyAsync(embed: reactionEmbed);
            }
        }
        #endregion
    }
}