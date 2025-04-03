using QuestGraph.Core.Protocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Telerik.Windows.Controls;
using Telerik.Windows.Controls.Diagrams;

namespace QuestGraph.Core.TelerikDrawingModel
{
    public class Connector : RadDiagramConnector
    {
        public enum LinkDirection { Top, Bottom, Right, Left }

        public LinkDirection Direction => direction;
        public string BlockId => blockId;
        public NodeType BlockType => blockType;

        private string blockId;
        private LinkDirection direction;
        private readonly NodeType blockType;

        public Connector(LinkDirection direction, NodeType blockType, string blockId) 
        {
            this.direction = direction;
            this.blockType = blockType;
            this.blockId = blockId;
            Name = direction.ToString();
        }

        public bool IsCompatibilityAble(Connector connector)
        {
            return connector.direction != direction 
                && connector.blockType != blockType
                && connector.BlockId != blockId;   
        }
    }
}