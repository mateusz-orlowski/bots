using Atos.AI.Bot.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Atos.AI.Bot.Services
{
    public class ConfigurationService
    {
        private static List<LUISConfig> ConfiguredLUISModels
        {
            get
            {
                return LUISModels.Value;
            }
        }

        private static Lazy<List<LUISConfig>> LUISModels = new Lazy<List<LUISConfig>>(() =>
        {
            var _LUISModels = new List<LUISConfig>();

            // Get all the Keys from the config file for QnA Maker
            LUISConfig luisModel = new LUISConfig();
            luisModel.Key = "LUIS.en-us";
            luisModel.Value = "TODO: LUIS ModelId;AppSubscriptionKey";

            _LUISModels.Add(luisModel);


            return _LUISModels;
        });

        public static LUISConfig GetLUISModelInfo(string Lang = "en-us")
        {
            LUISConfig luisModel = new LUISConfig();

            var langEntries = ConfiguredLUISModels.Where(x => x.Lang == Lang).FirstOrDefault();

            // If entry for a specific lang, then return it
            if (!(langEntries is null))
            {
                luisModel = langEntries;
            }

            return luisModel;
        }

        public static QnAConfig GetQnAModelInfo(string Topic, string Lang = "en-us")
        {


            QnAConfig topicEntry = new QnAConfig();
            topicEntry.Key = "QnA.Atos.en-us";

            topicEntry.Value = "TODO: AppId;AppSubscriptionKey;InfoURL;CapabilityText";

            return topicEntry;
        }
    }
}