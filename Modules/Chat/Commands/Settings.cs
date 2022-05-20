using System;
using System.Threading.Tasks;
using Discord.Interactions;
using Discord.WebSocket;
using DiscordBot.Modules.Chat.Class;
using DiscordBot.Modules.Chat.Embeds;
using Microsoft.VisualBasic;

namespace DiscordBot.Modules.Chat.Commands;


public class Settings : InteractionModuleBase<ShardedInteractionContext>
{
    [SlashCommand("reaction-message-add", "Add a new Reaction Role message")]
    public async Task NewReactionMessage(SocketTextChannel destination, string content)
    {
        await DeferAsync();
        try
        {
            var result = destination.SendMessageAsync(embed: new ReactionRoleMessageEmbed(content).Build()).GetAwaiter();
            ReactionRoleMessage.AddMessage(result.GetResult().Id, destination.Guild.Id);
            await FollowupAsync(
                "Successfully created reaction message, please use ```/reaction-role-add``` to add roles to the message");
        }
        catch (Exception e)
        {
            await FollowupAsync("Couldn't create reaction message");
            Console.WriteLine(e);
        }
    }

    [SlashCommand("reaction-message-remove", "Remove a Reaction Role message")]
    public async Task RemoveReactionMessage(SocketTextChannel channel, UInt64 messageSnowflake)
    {
        await DeferAsync();
        try
        {
            ReactionRoleMessage.RemoveMessage(messageSnowflake, channel.Guild.Id);
            
            await channel.DeleteMessageAsync(messageSnowflake);
            await FollowupAsync("Successfully removed reaction message");
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
    }
}