using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Atos.AI.Bot.Models
{
    [Serializable]
    public class GenericSearchResult
    {
        public string id { get; set; }
        public string name { get; set; }
        public string url { get; set; }
        public string displayUrl { get; set; }
        public string snippet { get; set; }
        public DateTime dateLastCrawled { get; set; }
    }
}