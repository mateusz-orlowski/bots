using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Atos.AI.Bot.Models
{
    [Serializable]
    public class LUISConfig
    {
        public string Key { get; set; }     // FORMAT -> LUIS.Lang

        public string Value { get; set; }   // FORMAT -> MOdelId;AppSubscriptionKey

        public string ModelId
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

        public string SubscriptionKey
        {
            get
            {
                string retVal = "";

                if (!(this.Value is null) && (this.Value.IndexOf(";") > 0))
                {
                    retVal = this.Value.Substring(this.Value.IndexOf(";") + 1);
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
                    retVal = this.Key.Replace("LUIS.", "");
                }

                return retVal;

            }
        }
    }
}