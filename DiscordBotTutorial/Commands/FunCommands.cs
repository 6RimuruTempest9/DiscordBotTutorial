﻿using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
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
    }
}