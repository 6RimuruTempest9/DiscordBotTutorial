using DSharpPlus;
using DSharpPlus.Entities;

namespace DiscordBotTutorial.Handlers.Dialogue.Steps
{
    public interface IDialogueStep
    {
        public Action<DiscordMessage> OnMessageAdded { get; set; }

        public IDialogueStep NextStep { get; }

        public Task<bool> ProcessStep(DiscordClient client, DiscordChannel channel, DiscordUser user);
    }
}