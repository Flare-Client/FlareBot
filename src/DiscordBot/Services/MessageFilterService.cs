using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DiscordBot.Services
{
    public class MessageFilterService
    {
        private readonly DiscordSocketClient _discord;
        private readonly CommandService _commands;
        private IServiceProvider _provider;

        public MessageFilterService(IServiceProvider provider, DiscordSocketClient discord, CommandService commands)
        {
            _discord = discord;
            _commands = commands;
            _provider = provider;

            _discord.MessageReceived += MessageReceived;
        }

        public async Task InitializeAsync(IServiceProvider provider)
        {
            _provider = provider;
            await _commands.AddModulesAsync(Assembly.GetEntryAssembly(), provider);
            // Add additional initialization code here...
        }

        private async Task MessageReceived(SocketMessage rawMessage)
        {
            string[] blacklist = File.ReadAllLines("WordBlacklist.txt");
            foreach(string bad in blacklist)
            {
                if (rawMessage.Content.ToLower().Contains(bad.ToLower()))
                {
                    rawMessage.DeleteAsync().ConfigureAwait(false).GetAwaiter().GetResult();
                }
            }
        }
    }
}
