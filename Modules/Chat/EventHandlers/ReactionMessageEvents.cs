using System;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using DiscordBot.Modules.Chat.Class;

namespace DiscordBot.Modules.Chat.EventHandlers;

public static class ReactionMessageEvents
{
    public static Task ReactionAdded(Cacheable<IUserMessage, ulong> message, ISocketMessageChannel channel,
        SocketReaction reaction)
    {
        SocketTextChannel textChannel = (SocketTextChannel)channel;

        if (ReactionRoleMessage.IsReactionMessage(message.Id))
        {
            ReactionRoleMessage.AddRole(reaction, message.Id, textChannel.Guild);
        }
        return Task.CompletedTask;
    }

    public static Task ReactionRemoved(Cacheable<IUserMessage, ulong> message, ISocketMessageChannel channel,
        SocketReaction reaction)
    {
        SocketTextChannel textChannel = (SocketTextChannel)channel;

        if (ReactionRoleMessage.IsReactionMessage(message.Id))
        {
            ReactionRoleMessage.RemoveRole(reaction, message.Id,textChannel.Guild);
        }
        return Task.CompletedTask;
    }
}