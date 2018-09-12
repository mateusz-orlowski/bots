using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace Atos.AI.Bot.Services
{
    public static class BotExtensions
    {

        public static async Task ShowBotBusy(this Activity activity)
        {
            var connector = new ConnectorClient(new Uri(activity.ServiceUrl));
            Activity isTypingReply = activity.CreateReply();
            isTypingReply.Type = ActivityTypes.Typing;
            await connector.Conversations.ReplyToActivityAsync(isTypingReply);
        }
        public static async Task ShowBotBusy(this IDialogContext context)
        {
            var reply = context.MakeMessage();
            reply.Type = ActivityTypes.Typing;
            await context.PostAsync(reply);
        }
    }
}