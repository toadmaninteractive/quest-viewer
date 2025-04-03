using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuestGraph.Core
{
    public class Constants
    {
        public const string LocalStorageFileExtension = ".json";
        public const string GraphStatesFolderName = "GraphStates";
        public const string GraphCustomPresetsFileName = "CustomPresets";
        public const string BlockGroupFileName = "BlockGroups";

        public static class Numerical
        {
            public const double BlockDefaultWidth = 200;
            public const double BlockMinWidth = 100;
            public const double BlockMinHeight = 25;

            public const double BlockGroupDefaultWidth = 150;
            public const double BlockGroupDefaultHeight = 110;
            public const double BlockGroupMinWidth = 100;
            public const double BlockGroupMinHeight = 25;
        }
    }
}