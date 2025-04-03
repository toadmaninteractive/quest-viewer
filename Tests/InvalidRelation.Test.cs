using NUnit.Framework;
using QuestGraph.Core.DomenModel;
using QuestViewer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests
{
    public class InvalidRelationTest
    {
        [Test]
        public void BuildBlockRelations_BlockDuplicate()
        {
            /*
            var source = DiagramModelConstructor.InvalidRelation_BlockDuplicate();
            var questGraph = new QuestGraphDomenModel();
            var relations = questGraph.GetBlockRelations(source.Edges, source.Blocks);
            Assert.AreEqual(relations.Count, 3);
            Assert.AreEqual(relations.ElementAt(0).Key, "1");
            Assert.AreEqual(relations.ElementAt(1).Key, "2");
            Assert.AreEqual(relations.ElementAt(2).Key, "3");*/
        }

        [Test]
        public void BuildBlockRelations_InvalidEdge()
        {
            /*var source = DiagramModelConstructor.InvalidRelation_InvalidEdge();
            var questGraph = new QuestGraphDomenModel();
            var relations = questGraph.GetBlockRelations(source.Edges, source.Blocks);
            Assert.AreEqual(relations.Count, 3);
            Assert.AreEqual(relations.ElementAt(0).Key, "1");
            Assert.AreEqual(relations.ElementAt(1).Key, "2");
            Assert.AreEqual(relations.ElementAt(2).Key, "3");*/
        }
    }
}