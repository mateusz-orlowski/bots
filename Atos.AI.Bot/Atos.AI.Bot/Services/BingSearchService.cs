using Atos.AI.Bot.Models;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;

namespace Atos.AI.Bot.Services
{
    public class BingSearchService
    {
        private const string BingSearchURL = "https://api.cognitive.microsoft.com/bing/v7.0/search?q='{0}'&mkt='{1}'&count=5&safesearch=Moderate";
        public static async Task DisplaySearchResults(string searchQuery, string language, IDialogContext context)
        {
            List<GenericSearchResult> bingSearchResults = new List<GenericSearchResult>();

            // Create the Search URL
            string searchURL = string.Format(BingSearchURL, searchQuery, language);

            try
            {
                // Execute the HTTP Request to get the results
                var searchResults = await WebAPIBaseService.ExecuteWebAPIRequest(HttpMethod.Get, searchURL, "Ocp-Apim-Subscription-Key", "TODO: Bing Search Key");

                // Retrieve the reults
                if ((searchResults != null) && !string.IsNullOrEmpty(searchResults.ReponseContent))
                {
                    JArray searchDetails = JObject.Parse(searchResults.ReponseContent.ToString())["webPages"]["value"] as JArray;
                    if (searchDetails != null)
                        bingSearchResults = searchDetails.ToObject<List<GenericSearchResult>>();
                }

                if (bingSearchResults.Count() > 0) // Check to see if there are any records
                {
                    var resultMessage = context.MakeMessage();
                    resultMessage.AttachmentLayout = AttachmentLayoutTypes.Carousel;
                    resultMessage.Attachments = new List<Attachment>();

                    // Loop through the resultset and create the cards
                    for (int indx = 0; indx < bingSearchResults.Count; indx++)
                    {
                        var bingSearchResult = bingSearchResults[indx];

                        HeroCard heroCard = new HeroCard()
                        {
                            Title = bingSearchResult.name,
                            Subtitle = bingSearchResult.displayUrl,
                            Text = bingSearchResult.snippet,
                            Buttons = new List<CardAction>()
                                {
                                    new CardAction()
                                    {
                                        Title = "Bing results",
                                        Type = ActionTypes.OpenUrl,
                                        Value = bingSearchResult.url
                                    }
                                }
                        };

                        resultMessage.Attachments.Add(heroCard.ToAttachment());

                    }

                    //await context.PostAsync(Resources.GenericSearch_TitleMessage);
                    await context.SayAsync("Maybe you'll find it useful:", "Maybe these results will help you");
                    await context.PostAsync(resultMessage);

                }
            }
            catch (Exception ex)
            {
                //LoggerService.Exception(ex, null, (Activity)context.Activity);
            }

        }
    }
}