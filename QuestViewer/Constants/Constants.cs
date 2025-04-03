using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuestViewer
{
    public class Constants
    {
        public const int CheckUpdateDefaultInterval = 1000 * 30; // msec
        public const int CheckFileChangesInterval = 1000; // msec
        public const string UpdateUrl = @"https://update.yourcompany.com/public/update/quest_viewer/";

        public const string ChronosApiKey = "TOPSECRET";
        public const string ChronosApiUri = "https://chronos.yourcompany.com:8090";
        public const string ChronosWsUri = "wss://chronos.yourcompany.com:8090/ws";
    }
}