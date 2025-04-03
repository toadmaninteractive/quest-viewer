using NUnit.Framework;
using QuestGraph.Core.DomenModel;
using QuestGraph.Core.Protocol;
using QuestViewer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests
{
    public class BlockRelationDictionaryTest
    {
        [SetUp]
        public void SetUp()
        {
        }

        [Test]
        public void BuildBlockRelations()
        {
            /*
            var source = DiagramModelConstructor.RelationTestDiagram();
            var questGraph = new QuestGraphDomenModel();
            var blocks = source.Blocks.Select(blockProtocolModel =>
            {
                switch (blockProtocolModel.Type)
                {
                    case BlockType.State:
                        return new QuestGraph.Core.DomenModel.BlockState((Protocol.BlockState)blockProtocolModel, drawingFactory) as IBlock;
                    case BlockType.Action:
                        return new QuestGraph.Core.DomenModel.BlockAction((Protocol.BlockAction)blockProtocolModel, drawingFactory);
                    default:
                        throw ExceptionConstructor.UnexpectedEnumValue(blockProtocolModel.Type, Environment.StackTrace);
                }
            })
            var relations = questGraph.GetBlockRelations(source.Edges.Select(x => new Edge(x)), source.Blocks.Select(x => x));
            */
        }
    }
}