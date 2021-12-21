using Discord;
using Discord.Interactions;

namespace DiscordBot.Modules.Commands.SlashCommands;

public class Warn : SlashCommandBuilder
{
    public Warn()
    {
        WithName("warn");
        WithDescription("Warn a user");
        AddOption("user", ApplicationCommandOptionType.User, "the user to warn", true);
        AddOption("reason", ApplicationCommandOptionType.String, "the reason for the warn", true);
    }
}