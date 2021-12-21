using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;

namespace DiscordBot.Modules;

public class CommandHandler
{

    public static async Task HandleSlashCommandAsync(SocketSlashCommand command)
    {
        await command.RespondAsync($"You executed {command.Data.Name}");
    }
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="arg"></param>
    /// <returns></returns>
    public static async Task HandleCommandAsync(SocketMessage arg)
    {
        CommandService Command = DiscordBot.Services.GetRequiredService<CommandService>();
        
        
        var message = arg as SocketUserMessage;
        ShardedCommandContext context = new ShardedCommandContext(DiscordBot.Services.GetRequiredService<DiscordShardedClient>(), message);
        if (message.Author.IsBot) return;

        int argPos = 0;
        if (message.HasStringPrefix("$", ref argPos))
        {
            var result = await Command.ExecuteAsync(context, argPos, DiscordBot.Services);

            if (!result.IsSuccess)
            {
                Embed exceptionEmbed = new EmbedBuilder()
                    .WithColor(Color.Red)
                    .WithDescription(result.ErrorReason)
                    .Build();

                await context.Channel.SendMessageAsync(embed: exceptionEmbed);
            }

            if (result.IsSuccess)
            {
                await EventHandlers.LogExecutedCommand(context, message);
            }
        }
    }

}