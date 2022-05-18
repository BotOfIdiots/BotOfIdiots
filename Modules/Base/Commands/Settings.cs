using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using DiscordBot.Database;
using MySql.Data.MySqlClient;

namespace DiscordBot.Modules.Base.Commands
{
    [RequireBotPermission(GuildPermission.ManageGuild)]
    [RequireUserPermission(GuildPermission.ManageGuild)]
    [Group("settings", "Command for changing various settings for the server")]
    public class Settings : InteractionModuleBase<ShardedInteractionContext>
    {
        public DatabaseService DatabaseService { get; set; }


        /// <summary>
        /// 
        /// </summary>
        [Group("join-role", "Manage the join role for the server")]
        class Join : Settings
        {
            /// <summary>
            /// 
            /// </summary>
            /// <param name="role"></param>
            [SlashCommand("set", "Set the role to be applied when a user joins the server")]
            public async Task set(IRole role)
            {
                string query = "UPDATE guild_configurations  SET JoinRole = @Role WHERE Guild = @Guild";

                MySqlParameter guild = new MySqlParameter("@Guild", MySqlDbType.UInt64);
                guild.Value = role.Guild.Id;

                MySqlParameter joinRole = new MySqlParameter("@Role", MySqlDbType.UInt64);
                joinRole.Value = role.Id;

                int succes = DatabaseService.ExecuteNonQuery(query, guild, joinRole);

                Embed reactionEmbed = (succes == 1)
                    ? new EmbedBuilder
                        {
                            Color = Color.Green
                        }
                        .WithDescription("Join role successfully set to: " + role.Mention)
                        .Build()
                    : new EmbedBuilder
                        {
                            Color = Color.Red
                        }
                        .WithDescription("Could not set join role to: " + role.Mention)
                        .Build();

                await RespondAsync(embed: reactionEmbed);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        [Group("moderation-role", "Manage the moderation role for the server")]
        class ModerationRole : Settings
        {
            [SlashCommand("set", "Set the role that's use for moderation in the server")]
            public async Task set(IRole role)
            {
                string query = "UPDATE guild_configurations  SET ModerationRole = @Role WHERE Guild = @Guild";

                MySqlParameter guild = new MySqlParameter("@Guild", MySqlDbType.UInt64);
                guild.Value = role.Guild.Id;

                MySqlParameter joinRole = new MySqlParameter("@Role", MySqlDbType.UInt64);
                joinRole.Value = role.Id;

                int succes = DatabaseService.ExecuteNonQuery(query, guild, joinRole);

                Embed reactionEmbed = (succes == 1)
                    ? new EmbedBuilder
                        {
                            Color = Color.Green
                        }
                        .WithDescription("Moderation role succesfully set to: " + role.Mention)
                        .Build()
                    : new EmbedBuilder
                        {
                            Color = Color.Red
                        }
                        .WithDescription("Could not set moderation role to: " + role.Mention)
                        .Build();

                await RespondAsync(embed: reactionEmbed);
            }
        }


        /// <summary>
        /// Manage the logging module
        /// </summary>
        [Group("logging", "Manage the logging module")]
        class Logging : Settings
        {
            [SlashCommand("enable", "Enables the logging module")]
            public async Task enable(SocketTextChannel logsChannel, SocketTextChannel exceptionsChannel)
            {
                string query =
                    "INSERT INTO log_channels_settings (Guild, Logs, Exceptions) VALUE (@Guild, @Logs, @Exceptions)";

                #region SQL Parameters

                MySqlParameter guild = new MySqlParameter("@Guild", MySqlDbType.UInt64);
                guild.Value = logsChannel.Guild.Id;

                MySqlParameter logs = new MySqlParameter("@Logs", MySqlDbType.UInt64);
                logs.Value = logsChannel.Id;

                MySqlParameter exceptions = new MySqlParameter("@Exceptions", MySqlDbType.UInt64);
                exceptions.Value = exceptionsChannel.Id;

                #endregion

                int succes = DatabaseService.ExecuteNonQuery(query, guild, logs, exceptions);

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

            [SlashCommand("set-channel", "set the logchannels for the guild")]
            public async Task set(string logType, SocketTextChannel logChannel)
            {
                string query = "UPDATE log_channels_settings SET " + logType + " = @logs WHERE Guild = @Guild";

                MySqlParameter guild = new MySqlParameter("@Guild", MySqlDbType.UInt64);
                guild.Value = logChannel.Guild.Id;

                MySqlParameter channel = new MySqlParameter("@Logs", MySqlDbType.UInt64);
                channel.Value = logChannel.Id;

                int succes = DatabaseService.ExecuteNonQuery(query, channel, guild);

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
        }


        [Group("modules", "Manage all the bot modules for this guild")]
        class Modulues : Settings
        {
            [SlashCommand("Enable", "Enable a module for this guild")]
            public async Task Enable(Enum module)
            {
                
            }
        }
        
        
        /// <summary>
        /// Manage the private channel module
        /// </summary>
        [Group("private-channel", "Manage the private channel module")]
        class PrivateChannel : Settings
        {
            /// <summary>
            /// Enable the Private Channel module for this server
            /// </summary>
            /// <param name="categorySnowflake"></param>
            /// <param name="voiceChannelSnowflake"></param>
            public async Task Enable(ulong categorySnowflake, ulong voiceChannelSnowflake)
            {
                String query = "INSERT INTO private_channels_setups VALUE (@Guild, @Category, @Channel);";

                #region SQL Parameters

                MySqlParameter guild = new MySqlParameter("@Guild", MySqlDbType.UInt64) { Value = Context.Guild.Id };
                MySqlParameter category = new MySqlParameter("@Category", MySqlDbType.UInt64)
                    { Value = categorySnowflake };
                MySqlParameter channel = new MySqlParameter("@Channel", MySqlDbType.UInt64)
                    { Value = voiceChannelSnowflake };

                #endregion

                int succes = DatabaseService.ExecuteNonQuery(query, channel, guild, category);

                if (succes == 1)
                {
                    Embed reactionEmbed = new EmbedBuilder
                        {
                            Color = Color.Green
                        }
                        .WithDescription("Successfully enabled Private Channels")
                        .Build();

                    await RespondAsync(embed: reactionEmbed);
                }
            }

            /// <summary>
            /// Show the current settings for the private channel module
            /// </summary>
            [SlashCommand("list", "Show the current settings for the private channel module")]
            public async Task List()
            {
                string query = "SELECT CategoryId, CreateChannelId FROM private_channels_setups WHERE Guild = @Guild ";

                #region SQL Parameters

                MySqlParameter guild = new MySqlParameter("@Guild", MySqlDbType.UInt64) { Value = Context.Guild.Id };

                #endregion

                using MySqlDataReader reader = DbOperations.ExecuteReader(DatabaseService, query, guild);

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

            /// <summary>
            /// Set the channel for the private channel module to check for the creation of a new channel
            /// </summary>
            /// <param name="voiceChannel"></param>
            [SlashCommand("set-channel", "Set the creation Channel")]
            public async Task Channel(SocketVoiceChannel voiceChannel)
            {
                string query = "UPDATE private_channels_setups SET CreateChannelId = @Channel WHERE Guild = @Guild";

                #region SQL Parameters

                MySqlParameter guild = new MySqlParameter("@Guild", MySqlDbType.UInt64)
                    { Value = voiceChannel.Guild.Id };

                MySqlParameter channel = new MySqlParameter("@Channel", MySqlDbType.UInt64)
                    { Value = voiceChannel.Id };

                #endregion

                DatabaseService.ExecuteNonQuery(query, guild, channel);

                await Task.CompletedTask;
            }

            /// <summary>
            /// Set the category for the private channel module to check
            /// </summary>
            /// <param name="categoryChannel"></param>
            [SlashCommand("set-category", "Set the category to check")]
            public async Task Category(SocketCategoryChannel categoryChannel)
            {
                string query = "UPDATE private_channels_setups SET CreateChannelId = @Channel WHERE Guild = @Guild";

                #region SQL Parameters

                MySqlParameter guild = new MySqlParameter("@Guild", MySqlDbType.UInt64)
                    { Value = categoryChannel.Guild.Id };

                MySqlParameter category = new MySqlParameter("@Category", MySqlDbType.UInt64)
                    { Value = categoryChannel.Id };

                #endregion

                DatabaseService.ExecuteNonQuery(query, guild, category);

                await Task.CompletedTask;
            }


            /// <summary>
            /// Disable the private channel module
            /// </summary>
            [SlashCommand("disable", "Disable the private channel module")]
            public async Task Disable()
            {
                await RespondAsync();
            }
        }

        //TODO Fix type error
        // /// <summary>
        // /// Manage the reaction message module
        // /// </summary>
        // [Group("reaction-message", "Manage the reaction message module")]
        // class ReactionMessages : Settings
        // {
        //     /// <summary>
        //     /// 
        //     /// </summary>
        //     /// <param name="message"></param>
        //     [SlashCommand("add", "add a reaction message")]
        //     public async Task Set(SocketMessage message)
        //     {
        //         if (DbOperations.InsertReactionMessage(message.Id, Context.Guild))
        //         {
        //             Embed reactionMessageConfig = new ReactionMessageConfig(message).Build();
        //             await ReplyAsync(embed: reactionMessageConfig);
        //         }
        //
        //         await Task.CompletedTask;
        //     }
        //
        //     /// <summary>
        //     /// 
        //     /// </summary>
        //     /// <param name="message"></param>
        //     /// <exception cref="NotImplementedException"></exception>
        //     [SlashCommand("remove", "remove a reaction message")]
        //     public async Task Remove(SocketMessage message)
        //     {
        //         throw new NotImplementedException();
        //     }
        // }
    }
}