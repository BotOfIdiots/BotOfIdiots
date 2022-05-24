using System;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using DiscordBot.Database;
using MySql.Data.MySqlClient;

namespace DiscordBot.Discord.Preconditions;

public class UserIsDeveloper : PreconditionAttribute
{
    private string _errorMessage;

    public DatabaseService DbConnection { set; get; }

    public UserIsDeveloper( string errorMessage = null)
    {
        if (errorMessage != null)
        {
            _errorMessage = errorMessage;
        }
    }

    public override Task<PreconditionResult> CheckPermissionsAsync(ICommandContext context, CommandInfo command,
        IServiceProvider services)
    {
        if (context.Guild == null) return Task.FromResult(PreconditionResult.FromError("Unknown command."));

        if (context.Guild.Id == Bot.ControleGuild)
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
        
        using MySqlDataReader reader = DbOperations.ExecuteReader(DbConnection, query, guild);

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