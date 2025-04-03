using Microsoft.VisualBasic;
using QuestGraph.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace QuestViewer
{
    [XmlRoot(IsNullable = false)]
    public class Config
    {
        public static string UserNameFromCurrentConnection;

        public ApplicationUpdateChannel UpdateChannel { get; set; }
        public DrawingConfiguration DrawingConfig { get; set; }
        public LastConnection LastConnection { get; set; }

        public Config()
        {
            DrawingConfig = new DrawingConfiguration(BlockInteractionMode.Viewer, BlockConnectorMode.TopBottom, EdgesRoutingMode.AStarRouting);//Default value
        }

        public static Config Instance => SerializationUtils.LoadXml<Config>(FileAndFolderNames.ConfigXmlFilePath);

        public void Save()
        {
            SerializationUtils.Serialize<Config>(FileAndFolderNames.ConfigXmlFilePath, this);
        }

        public void UpdateEdgesRoutingMode(EdgesRoutingMode edgesRoutingMode)
        {
            DrawingConfig = new DrawingConfiguration(BlockInteractionMode.Viewer, DrawingConfig.ConnectorMode, edgesRoutingMode);
            Save();
        }
    }

    public class LastConnection
    {
        public string DatabaseName { get; set; }
        public string Url { get; set; }
        public string DocumentId { get; set; }

        public LastConnection() { }

        public LastConnection(string databaseName, string url, string documentId)
        {
            DatabaseName = databaseName;
            Url = url;
            DocumentId = documentId;
        }
    }
}