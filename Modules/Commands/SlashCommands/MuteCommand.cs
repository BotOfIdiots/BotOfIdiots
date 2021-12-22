using Discord;

namespace DiscordBot.Modules.Commands.SlashCommands;

public class MuteCommand : SlashCommandBuilder
{
    public MuteCommand()
    {
        WithName("mute");
        WithDescription("Mute a user in the guild");
        AddOption("user", ApplicationCommandOptionType.User, "The user to mute", true);
        AddOption("reason", ApplicationCommandOptionType.String, "The reason for the mute", true);
    }
}