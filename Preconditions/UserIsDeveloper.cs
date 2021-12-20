using System;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using DiscordBot.Database;
using MySql.Data.MySqlClient;

namespace DiscordBot.Preconditions;

public class UserIsDeveloper : PreconditionAttribute
{
    private readonly ulong _controleGuild;
    private string _errorMessage;

    public UserIsDeveloper(string errorMessage = null)
    {
        _controleGuild = DiscordBot.ControleGuild;
        if (errorMessage != null)
        {
            _errorMessage = errorMessage;
        }
    }

    public override Task<PreconditionResult> CheckPermissionsAsync(ICommandContext context, CommandInfo command,
        IServiceProvider services)
    {
        if (context.Guild == null) return Task.FromResult(PreconditionResult.FromError("Unknown command."));

        if (context.Guild.Id == _controleGuild)
        {
            if (context.User.Id == 140789549092569088 || CheckUser(context.User))
            {
                return Task.FromResult(PreconditionResult.FromSuccess());
            }
        }
        
        return Task.FromResult(PreconditionResult.FromError("Unknown command."));
    }

    private bool CheckUser(IUser user)
    {
        String query = "SELECT Snowflake FROM developers WHERE Snowflake = @UserId;";

        MySqlParameter guild = new MySqlParameter("@UserId", MySqlDbType.UInt64) { Value = user.Id };

        DiscordBot.DbConnection.CheckConnection();
        using MySqlConnection conn = DiscordBot.DbConnection.SqlConnection;
        MySqlDataReader reader = DbOperations.ExecuteReader(conn, query, guild);

        while (reader.Read())
        {
            if (reader.GetUInt64("Snowflake") == user.Id)
            {
                return true;
            }
        }

        return false;
    }
}