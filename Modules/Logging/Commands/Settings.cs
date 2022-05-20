using System.Threading.Tasks;
using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using DiscordBot.Database;
using MySql.Data.MySqlClient;

namespace DiscordBot.Modules.Logging.Commands;

[Group("logging", "Change all the log settings")]
public class Settings : InteractionModuleBase<ShardedInteractionContext>
{
    public DatabaseService DatabaseService { get; set; }
    
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