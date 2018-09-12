using System;
using System.Threading.Tasks;
using System.Web;
using Atos.AI.Bot.Services;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;

namespace Atos.AI.Bot.Dialogs
{
    [Serializable]
    public class RootDialog : IDialog<object>
    {
        public Task StartAsync(IDialogContext context)
        {
            context.Wait(MessageReceivedAsync);

            return Task.CompletedTask;
        }

        private async Task MessageReceivedAsync(IDialogContext context, IAwaitable<object> result)
        {
            // Show visual clue to the user
            await context.ShowBotBusy();
            var activity = await result as Activity;
            // A class for getting ModelId and SubsriptionKey from the config file
            var luisModelInfo = ConfigurationService.GetLUISModelInfo();
            var luisDialog = new Dialogs.RootLuisDialog(luisModelInfo.ModelId, luisModelInfo.SubscriptionKey);
            if (!(luisDialog is null))
                await context.Forward(luisDialog, AfterMessageReceivedLUISAsync, activity);

            // calculate something for us to return
            //int length = (activity.Text ?? string.Empty).Length;

            //// return our reply to the user
            //await context.PostAsync($"You sent {activity.Text} which was {length} characters");

            //context.Wait(MessageReceivedAsync);
        }

        private async Task AfterMessageReceivedLUISAsync(IDialogContext context, IAwaitable<object> result)
        {
            try
            {
                // Remove the Dialog from the stack
                context.Done<object>(null);
            }
            catch (Exception ex)
            {
                //LoggerService.Exception(ex, null, (Activity)context.Activity);
            }
        }
    }
}