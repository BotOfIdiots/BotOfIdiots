using System;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;

namespace DiscordBot.Modules
{
    public class Levels : ModuleBase<SocketCommandContext>
    {
        public static Task AddReactionXp(Cacheable<IUserMessage, ulong> cacheable, ISocketMessageChannel socketMessageChannel, SocketReaction arg3)
        {
            throw new NotImplementedException();
        }
        
        public static Task RemoveReactionXp(Cacheable<IUserMessage, ulong> cacheable, ISocketMessageChannel socketMessageChannel, SocketReaction arg3)
        {
            throw new NotImplementedException();
        }

        public static Task AddMessageXp(SocketMessage socketMessage)
        {
            throw new NotImplementedException();
        }

        public static Task ResetXp()
        {
            throw new NotImplementedException();
        }

        public static Task FreezeXp()
        {
            throw new NotImplementedException();
        }
        
    }
}