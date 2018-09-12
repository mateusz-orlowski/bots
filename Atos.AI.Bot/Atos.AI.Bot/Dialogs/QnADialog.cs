using Atos.AI.Bot.Services;
using Microsoft.Bot.Builder.CognitiveServices.QnAMaker;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace Atos.AI.Bot.Dialogs
{
    [Serializable]
    public class QnADialog : QnAMakerDialog
    {
        private string _topic;

        public QnADialog(string topic, string subscriptionKey, string knowledgeBaseId, string defaultMessage = null, double scoreThreshhold = 0.3, int top = 1) :
            base(CreateQnAService(subscriptionKey, knowledgeBaseId, defaultMessage, scoreThreshhold, top))
        {
            this._topic = topic;
        }

        private static IQnAService CreateQnAService(string subscriptionKey, string knowledgeBaseId, string defaultMessage = null, double scoreThreshhold = 0.3, int top = 1)
        {
            var qnaAttribute = new QnAMakerAttribute(subscriptionKey, knowledgeBaseId, defaultMessage, scoreThreshhold, top);
            IQnAService qnAService = new QnAMakerService(qnaAttribute);
            return qnAService;
        }

        //// Override to also include the knowledgebase question with the answer on confident matches
        protected override async Task RespondFromQnAMakerResultAsync(IDialogContext context, IMessageActivity message, QnAMakerResults qnaMakerResults)
        {
            string language = "en-us";

            if (qnaMakerResults.Answers.Count > 0 && qnaMakerResults.Answers.FirstOrDefault().Score >= 0.5)
            {
                await PostReplyToUser(context, qnaMakerResults);
            }
            else
            {
                var userFeedbackMessage = string.Format("I know nothing about {0}. Let me goog... err, bing it.", message.Text);

                // Post the No answer found message to the user
                await context.SayAsync(userFeedbackMessage, userFeedbackMessage);

                // Show generic Help (Bing Search)
                await BingSearchService.DisplaySearchResults(((Activity)context.Activity).Text, language, context);

            }
        }

        //// Override to log matched Q&A before ending the dialog
        protected override async Task DefaultWaitNextMessageAsync(IDialogContext context, IMessageActivity message, QnAMakerResults qnaMakerResults)
        {
            LogAndLoadFeedbackDialog(context, qnaMakerResults);
        }

        //// Override to enable Q&A active learning
        protected override async Task QnAFeedbackStepAsync(IDialogContext context, QnAMakerResults qnaMakerResults)
        {

            // responding with the top answer when score is above some threshold
            if (qnaMakerResults.Answers.Count > 0 && qnaMakerResults.Answers.FirstOrDefault().Score >= 0.8)
            {
                await PostReplyToUser(context, qnaMakerResults);

                // Log and load the feedback dialog
                LogAndLoadFeedbackDialog(context, qnaMakerResults);

            }
            else
            {
                await base.QnAFeedbackStepAsync(context, qnaMakerResults);
            }
        }

        private void LogAndLoadFeedbackDialog(IDialogContext context, QnAMakerResults qnaMakerResults)
        {

            var userQuestion = (context.Activity as Activity).Text;

            string botAnswer = "";
            double answerConfidence = 0;
            bool exitedInner = false;

            // Check only for condition, when the control has come back from QnAFeedbackStep
            if (qnaMakerResults.Answers.Count > 1)
            {
                foreach (var answer in qnaMakerResults.Answers)
                {
                    foreach (var question in answer.Questions)
                    {
                        if (userQuestion == question)
                        {
                            if (answer.Score < 0.8)
                                answerConfidence = 0.8; // Since the "answer.Score" reported by the SDK is incorrect
                            else
                                answerConfidence = answer.Score;

                            botAnswer = HttpUtility.HtmlDecode(answer.Answer);

                            exitedInner = true;
                            break;
                        }
                    }
                    if (exitedInner)
                        break;
                }
            }

            if (answerConfidence == 0 && qnaMakerResults.Answers.Count > 0)
                answerConfidence = qnaMakerResults.Answers.FirstOrDefault().Score;

            if (qnaMakerResults.Answers.Count > 0 && answerConfidence >= 0.5)
            {
                if (string.IsNullOrEmpty(botAnswer))
                    botAnswer = HttpUtility.HtmlDecode(qnaMakerResults.Answers.FirstOrDefault().Answer);

                // Log Bot usage - Answer found
                //context.LogEvent("QnA", userProfile, new Dictionary<string, string> { { @"Intent", "MatchFound" }, { @"UserMsg", userQuestion }, { @"BotAnswer", botAnswer }, { @"AnswerConfidence", answerConfidence.ToString() } });
            }
            else
            {
                // Log Bot usage - No answer
                //context.LogEvent("QnA", BotApp.CurrentUserProfile, new Dictionary<string, string> { { @"Intent", "NoMatch" }, { @"UserMsg", userQuestion }, { @"AnswerConfidence", answerConfidence.ToString() } });
            }

            // pass user's question to the feedback dialog
            //context.Call(new FeedbackDialog(userQuestion, botAnswer, answerConfidence.ToString()), ResumeAfterFeedback);
        }

        private async Task PostReplyToUser(IDialogContext context, QnAMakerResults qnaMakerResults)
        {
            var returnMessage = context.MakeMessage();

            var qnaReply = HttpUtility.HtmlDecode(qnaMakerResults.Answers.FirstOrDefault().Answer);

            // Check to see if any video content exists, if so, add as attachement
            

            returnMessage.Text = qnaReply;
            returnMessage.Speak = qnaReply;
            await context.PostAsync(returnMessage);
        }
        private async Task ResumeAfterFeedback(IDialogContext context, IAwaitable<IMessageActivity> result)
        {

            context.Done<IMessageActivity>(null);

        }

    }
}