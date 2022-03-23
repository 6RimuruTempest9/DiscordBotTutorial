using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity.Extensions;

namespace DiscordBotTutorial.Commands
{
    public class FunCommands : BaseCommandModule
    {
        [Command("ping")]
        [Description("Returns pong")]
        public async Task Ping(CommandContext context)
        {
            await context.Channel.SendMessageAsync("Pong").ConfigureAwait(false);
        }

        [Command("add")]
        [Description("Adds two number together")]
        public async Task Ping(CommandContext context,
            [Description("First number")] int firstNumber,
            [Description("Second number")] int secondNumber)
        {
            await context.Channel.SendMessageAsync((firstNumber + secondNumber).ToString()).ConfigureAwait(false);
        }

        [Command("messagecoping")]
        public async Task MessageCoping(CommandContext context)
        {
            var interactivity = context.Client.GetInteractivity();

            var message = await interactivity.WaitForMessageAsync(message => message.Channel == context.Channel).ConfigureAwait(false);

            if (message.TimedOut)
            {
                await context.Channel.SendMessageAsync("Timed out").ConfigureAwait(false);
            }
            else
            {
                await context.Channel.SendMessageAsync(message.Result.Content).ConfigureAwait(false);
            }
        }

        [Command("reactioncoping")]
        public async Task ReactionCoping(CommandContext context)
        {
            var interactivity = context.Client.GetInteractivity();

            var message = await interactivity.WaitForReactionAsync(message => message.Channel == context.Channel).ConfigureAwait(false);

            if (message.TimedOut)
            {
                await context.Channel.SendMessageAsync("Timed out").ConfigureAwait(false);
            }
            else
            {
                await context.Channel.SendMessageAsync(message.Result.Emoji).ConfigureAwait(false);
            }
        }

        [Command("myusername")]
        public async Task MyUsername(CommandContext context)
        {
            await context.Channel.SendMessageAsync(context.User.Username).ConfigureAwait(false);
        }

        [Command("getmessage")]
        public async Task GetMessage(CommandContext context, params string[] message)
        {
            await context.Member.SendMessageAsync(string.Join(" ", message)).ConfigureAwait(false);
        }

        [Command("poll")]
        public async Task Poll(CommandContext ctx, TimeSpan duration, params DiscordEmoji[] emojiOptions)
        {
            var interactivity = ctx.Client.GetInteractivity();
            var options = emojiOptions.Select(x => x.ToString());

            var pollEmbed = new DiscordEmbedBuilder
            {
                Title = "Poll",
                Description = string.Join(" ", options)
            };

            var pollMessage = await ctx.Channel.SendMessageAsync(embed: pollEmbed).ConfigureAwait(false);

            foreach (var option in emojiOptions)
            {
                await pollMessage.CreateReactionAsync(option).ConfigureAwait(false);
            }

            var result = await interactivity.CollectReactionsAsync(pollMessage, duration).ConfigureAwait(false);

            var results = result.Select(x => $"{x.Emoji}: {x.Total}");

            await ctx.Channel.SendMessageAsync(string.Join("\n", results)).ConfigureAwait(false);
        }
    }
}