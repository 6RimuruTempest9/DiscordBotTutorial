using DiscordBotTutorial.Commands;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.EventArgs;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Text;
using DSharpPlus.Interactivity;
using DSharpPlus.Interactivity.Extensions;

namespace DiscordBotTutorial
{
    public class Bot
    {
        public DiscordClient DiscordClient { get; private set; }
        public CommandsNextExtension CommandsNextExtension { get; private set; }

        public async Task RunAsync()
        {
            var json = string.Empty;

            using (var fileStream = File.OpenRead("config.json"))
            using (var streamReader = new StreamReader(fileStream, new UTF8Encoding(false)))
            {
                json = await streamReader.ReadToEndAsync().ConfigureAwait(false);
            }

            var configJson = JsonConvert.DeserializeObject<ConfigJson>(json);

            var discordConfiguration = new DiscordConfiguration
            {
                Token = configJson.Token,
                TokenType = TokenType.Bot,
                AutoReconnect = true,
                MinimumLogLevel = LogLevel.Debug,
            };

            DiscordClient = new DiscordClient(discordConfiguration);

            DiscordClient.Ready += OnClientReadyAsync;

            DiscordClient.UseInteractivity(new InteractivityConfiguration
            {
                Timeout = TimeSpan.FromMinutes(2),
            });

            var commandsNextConfiguration = new CommandsNextConfiguration
            {
                StringPrefixes = new[] { configJson.Prefix },
                EnableDms = false,
                DmHelp = true,
            };

            CommandsNextExtension = DiscordClient.UseCommandsNext(commandsNextConfiguration);

            CommandsNextExtension.RegisterCommands<FunCommands>();

            await DiscordClient.ConnectAsync();

            await Task.Delay(-1);
        }

        private Task OnClientReadyAsync(DiscordClient sender, ReadyEventArgs e)
        {
            return Task.CompletedTask;
        }
    }
}