using System.Collections.Generic;
using System.Linq;
using Discord.WebSocket;

namespace DiscordBot.Class;

public static class DeconstructionExtensions
{
    public static void Deconstruct(this IReadOnlyCollection<SocketSlashCommandDataOption> options, out object parameter1, out object parameter2)
    {
        SocketSlashCommandDataOption[] array = options.ToArray();

        parameter1 = array[0].Value;
        parameter2 = array[1].Value;
    }
}