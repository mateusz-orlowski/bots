using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Atos.AI.Bot.Models
{
    [Serializable]
    public class QnAConfig
    {
        public string Key { get; set; }     // FORMAT -> QnA.Topic.Lang

        public string Value { get; set; }   // FORMAT -> AppId;AppSubscriptionKey;InfoURL;CapabilityText

        public string AppId
        {
            get
            {

                string retVal = "";

                if (!(this.Value is null) && (this.Value.IndexOf(";") > 0))
                {
                    retVal = this.Value.Substring(0, this.Value.IndexOf(";"));
                }

                return retVal;
            }
        }

        public string AppSubscriptionKey
        {
            get
            {
                string retVal = "";
                string[] values = Value.Split(';');

                if (!(this.Value is null) && (this.Value.IndexOf(";") > 0) && (values.Count() > 1))
                {
                    retVal = values[1];
                }

                return retVal;
            }
        }

        public string MoreInfoURL
        {
            get
            {
                string retVal = "";
                string[] values = Value.Split(';');

                if (!(this.Value is null) && (this.Value.IndexOf(";") > 0) && (values.Count() > 2))
                {
                    retVal = values[2];
                }

                return retVal;
            }
        }

        public string CapabilityText
        {
            get
            {
                string retVal = "";
                string[] values = Value.Split(';');

                if (!(this.Value is null) && (this.Value.IndexOf(";") > 0) && (values.Count() > 3))
                {
                    retVal = values[3];
                }

                return retVal;
            }
        }

        public string Topic
        {
            get
            {
                string retVal = "";

                if (!(this.Key is null))
                {
                    string topicLangVal = this.Key.Replace("QnA.", "");

                    if (topicLangVal.IndexOf(".") > 0)
                    {
                        retVal = topicLangVal.Substring(0, topicLangVal.IndexOf(".")).Replace("-", " ");
                    }
                }

                return retVal;

            }
        }

        public string Lang
        {
            get
            {
                string retVal = "";

                if (!(this.Key is null))
                {
                    string topicLangVal = this.Key.Replace("QnA.", "");

                    if (topicLangVal.IndexOf(".") > 0)
                    {
                        retVal = topicLangVal.Substring(topicLangVal.IndexOf(".") + 1);
                    }
                }

                return retVal;
            }
        }
    }
}