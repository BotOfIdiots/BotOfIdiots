using Discord;

namespace DiscordBot.Modules.Commands.SlashCommands;

public class WarnCommand : SlashCommandBuilder
{
    public WarnCommand()
    {
        WithName("warn");
        WithDescription("Warn a user");
        AddOption("user", ApplicationCommandOptionType.User, "The user to warn", true);
        AddOption("reason", ApplicationCommandOptionType.String, "The reason for the warn", true);
    }
}