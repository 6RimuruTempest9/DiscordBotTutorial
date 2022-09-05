using DiscordBotTutorial.Handlers.Dialogue;
using DiscordBotTutorial.Handlers.Dialogue.Steps;
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

        [Command("dialogue")]
        public async Task Dialogue(CommandContext context)
        {
            var inputStep = new TextStep("Enter something interesting!", default, minLength: 10);
            var funnyStep = new IntStep("Haha, funny", default, maxValue: 100);

            string input = string.Empty;
            int value = 0;

            inputStep.OnValidResult += (result) =>
            {
                input = result;

                if (result == "something interesting")
                {
                    inputStep.SetNextStep(funnyStep);
                }
            };

            funnyStep.OnValidResult += (result) => value = result;

            var userChannel = await context.Member.CreateDmChannelAsync().ConfigureAwait(false);

            var inputDialogueHandler = new DialogueHandler(
                context.Client,
                userChannel,
                context.User,
                inputStep);

            var succeded = await inputDialogueHandler.ProcessDialogue().ConfigureAwait(false);

            if (!succeded)
            {
                return;
            }

            await context.Channel.SendMessageAsync(input).ConfigureAwait(false);

            await context.Channel.SendMessageAsync(value.ToString()).ConfigureAwait(false);
        }

        [Command("emojidialogue")]
        public async Task EmojiDialogue(CommandContext ctx)
        {
            var yesStep = new TextStep("You chose yes", null);
            var noStep = new IntStep("You chose no", null);

            var emojiStep = new ReactionStep("Yes Or No?", new Dictionary<DiscordEmoji, ReactionStepData>
            {
                { DiscordEmoji.FromName(ctx.Client, ":thumbsup:"), new ReactionStepData { Content = "This means yes", NextStep = yesStep } },
                { DiscordEmoji.FromName(ctx.Client, ":thumbsdown:"), new ReactionStepData { Content = "This means no", NextStep = noStep } }
            });

            var userChannel = await ctx.Member.CreateDmChannelAsync().ConfigureAwait(false);

            var inputDialogueHandler = new DialogueHandler(
                ctx.Client,
                userChannel,
                ctx.User,
                emojiStep
            );

            bool succeeded = await inputDialogueHandler.ProcessDialogue().ConfigureAwait(false);

            if (!succeeded) { return; }
        }
    }
}