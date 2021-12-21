using Discord;

namespace DiscordBot.Modules.Commands.SlashCommands;

public class Warn : SlashCommandBuilder
{
    public Warn()
    {
        WithName("warn");
        WithDescription("warn a user");
        AddOption("user", ApplicationCommandOptionType.User, "the user to warn", true);
        AddOption("reason", ApplicationCommandOptionType.String, "the reason for the warn", true);
    }
}