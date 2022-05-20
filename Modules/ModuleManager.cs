using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Discord.WebSocket;
using DiscordBot.Modules.Base;
using DiscordBot.Modules.Chat;
using DiscordBot.Modules.Event;
using DiscordBot.Modules.Logging;

namespace DiscordBot.Modules;

public class ModuleManager
{
    private readonly BaseModule _baseModule; 
    private readonly List<BaseModule> _modules;
    private DiscordShardedClient _client;
    private readonly IServiceProvider _services;


    public ModuleManager(DiscordShardedClient client, IServiceProvider services)
    {
        _client = client;
        _services = services;
        
        // _modules = new List<BaseModule>();
        _baseModule = new BaseModule(_client, _services);
        
        // BuildModuleList();
    }

    public async Task Initialize()
    {
        _client.ShardReady += Ready;
        await Task.CompletedTask;
    }

    private async Task Ready(DiscordSocketClient client)
    {
        new DiscordEventHooks(_client);
        _baseModule.Initialize().GetAwaiter();
// #if DEBUG
//         await EnableAllModules(317226837841281024);
//         await Task.CompletedTask;
// #else
//         await EnableAllModules(DiscordBot.ControleGuild);
//         
//         foreach (var guild in GetGuildList())
//         {
//             foreach (var module in GetEnabledModules())
//             {
//                 await EnableModule(guild, Type.GetType("Modules." + module));
//             }
//         }
// #endif

        _client.ShardReady -= Ready;
    }

    private List<String> GetEnabledModules()
    {
        throw new NotImplementedException();
    }

    private List<ulong> GetGuildList()
    {
        return null;
    }
    
    
    private void BuildModuleList()
    {
        //TODO: Automatically Discover all available modules
        _modules.Add(new LoggingModule(_client, _services));
        _modules.Add(new ChatModule(_client, _services));
    }

    private async Task EnableAllModules(ulong guildId)
    {
        foreach (BaseModule module in _modules)
        {
            await module.CommandHandler.RegisterCommandsAsync(guildId);
        }
    }

    public async Task EnableModule(ulong guildId, Type module)
    {
        if (module != typeof(BaseModule) && _modules.Count > 1)
        {
            try
            {
                await _modules.Find(x => x.GetType() == module)
                    .CommandHandler.RegisterCommandsAsync(guildId);
            }
            catch (NullReferenceException)
            {
                Console.WriteLine("Failed to enable module");
            }
        }
    }

    public async Task DisableModule(ulong guildId, Type module)
    {
        if (module != typeof(BaseModule) && _modules.Count > 1)
        {
            try
            {
                await _modules.Find(x => x.GetType() == module)
                    .CommandHandler.RegisterCommandsAsync(guildId);
            }
            catch (NullReferenceException)
            {
                Console.WriteLine("Failed to disable module");
            }
        }
    }
}