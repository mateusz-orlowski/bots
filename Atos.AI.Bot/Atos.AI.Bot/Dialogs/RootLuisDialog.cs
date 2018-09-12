using System;
using System.Threading;
using System.Threading.Tasks;
using Atos.AI.Bot.Services;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Internals.Fibers;
using Microsoft.Bot.Builder.Luis;
using Microsoft.Bot.Builder.Luis.Models;
using Microsoft.Bot.Connector;

namespace Atos.AI.Bot.Dialogs
{

    /// <summary>
    /// The top-level natural language dialog for sample.
    /// </summary>
    [Serializable]
    public class RootLuisDialog : LuisDialog<object>
    {
        public RootLuisDialog(string ModelId, string SubscriptionKey) : base(GetLuisService(ModelId, SubscriptionKey))
        {
        }

        private static ILuisService GetLuisService(string ModelId, string SubscriptionKey)
        {
            var luisModel = new LuisModelAttribute(ModelId, SubscriptionKey, LuisApiVersion.V2, "westus.api.cognitive.microsoft.com");
            ILuisService luisService = new LuisService(luisModel);
            return luisService;
        }

        [LuisIntent("")]
        [LuisIntent("None")]
        [LuisIntent("Search")]
        public async Task Search(IDialogContext context, IAwaitable<IMessageActivity> activity, LuisResult result)
        {
            try
            {
                var messageActivity = await activity;

                //Hand over to QnAMaker Service, based on the user topic/ language
                var qnaModelInfo = ConfigurationService.GetQnAModelInfo("Atos", "en-us");
                var qnadialog = new QnADialog(
                    qnaModelInfo.Topic,
                    qnaModelInfo.AppSubscriptionKey,
                    qnaModelInfo.AppId,
                    string.Format("Sorry, I couldn't find any specifc answer related to the topic {0}. Please try asking another question", qnaModelInfo.Topic),
                    0,
                    3
                );

                // Forward the request to the QnA Dialog 
                if (!(qnadialog is null))
                    await context.Forward(qnadialog, AfterMessageReceivedQnAAsync, messageActivity, CancellationToken.None);

            }
            catch (Exception ex)
            {
                
            }

        }

        [LuisIntent("Greeting")]
        public async Task Hello(IDialogContext context, IAwaitable<IMessageActivity> activity, LuisResult result)
        {
            try
            {
                var messageActivity = await activity;
                var returnMessage = context.MakeMessage();
                returnMessage.Text = "Hi there. What would you like to know about Atos Consulting?";
                returnMessage.Speak = "Hi there. What would you like to know about Atos Consulting?";
                await context.PostAsync(returnMessage);

                // Remove the Dialog from the stack
                //context.Done<object>(null);
            }
            catch (Exception ex)
            {

            }
        }



        private async Task ResumeAfterORDialog(IDialogContext context, IAwaitable<object> result)
        {
            try
            {

                // Remove the OR Dialog from the stack
                context.Done<object>(null);

            }
            catch (Exception ex)
            {
               
            }

        }

        private async Task AfterMessageReceivedQnAAsync(IDialogContext context, IAwaitable<object> result)
        {
            try
            {

                // Remove the QnADialog from the stack
                context.Done<object>(null);

            }
            catch (Exception ex)
            {
              
            }

        }
    }
}