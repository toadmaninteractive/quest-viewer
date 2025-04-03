using NUnit.Framework;
using QuestViewer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests
{
    public class TcpServerTest
    {
        [Test]
        public void EmptySplitterTest()
        {
            var tcpRequestSplitter = new EmptySymbolSplitter();
            var textRequest = "{ \"type\": \"dfg\", \"db\": \"moomin-dev-vladimir\", \"doc_id\": \"main_graph\", \"nodes\":\r\n\t{\r\n\t\t\"1555289e-6111-4453-a325-0b1bff427388\": true\r\n\t\t\"bb713679-9e3a-40c5-8cd3-f7dcb28a9642\": true\r\n\t}}{ \"type\": \"sdc\", \"db\": \"moomin-dev-vladimir\", \"doc_id\": \"main_graph\" }{ \"type\": \"zxc\", \"db\": \"moomin-dev-vladimir\", \"doc_id\": \"main_graph\", \"nodes\":\r\n\t{\r\n\t\t\"1555289e-6111-4453-a325-0b1bff427388\": true\r\n\t\t\"bb713679-9e3a-40c5-8cd3-f7dcb28a9642\": true\r\n\t}}{ \"type\": \"dv\", \"db\": \"moomin-dev-vladimir\", \"doc_id\": \"main_graph\" }{ \"type\": \"dfv\", \"db\": \"moomin-dev-vladimir\", \"doc_id\": \"main_graph\" }{ \"type\": \"df\", \"db\": \"moomin-dev-vladimir\", \"doc_id\": \"main_graph\" }{ \"type\": \"vz\", \"db\": \"moomin-dev-vladimir\", \"doc_id\": \"main_graph\" }{ \"type\": \"xc\", \"db\": \"moomin-dev-vladimir\", \"doc_id\": \"main_graph\" }{ \"type\": \"dv\", \"db\": \"moomin-dev-vladimir\", \"doc_id\": \"main_graph\" }{ \"type\": \"fdv\", \"db\": \"moomin-dev-vladimir\", \"doc_id\": \"main_graph\" }{ \"type\": \"sd\", \"db\": \"moomin-dev-vladimir\", \"doc_id\": \"main_graph\" }";
            var requestArray = tcpRequestSplitter.Split(textRequest);
        }

        [Test]
        public void UnicodeSplitterTest()
        {
            var tcpRequestSplitter = new UnicodeSymbolSplitter();
            var textRequest = "{ \"type\": \"dfg\", \"db\": \"moomin-dev-vladimir\", \"doc_id\": \"main_graph\", \"nodes\":\r\n\t{\r\n\t\t\"1555289e-6111-4453-a325-0b1bff427388\": true\r\n\t\t\"bb713679-9e3a-40c5-8cd3-f7dcb28a9642\": true\r\n\t}}\u0001{ \"type\": \"sdc\", \"db\": \"moomin-dev-vladimir\", \"doc_id\": \"main_graph\" }\u0001{ \"type\": \"zxc\", \"db\": \"moomin-dev-vladimir\", \"doc_id\": \"main_graph\", \"nodes\":\r\n\t{\r\n\t\t\"1555289e-6111-4453-a325-0b1bff427388\": true\r\n\t\t\"bb713679-9e3a-40c5-8cd3-f7dcb28a9642\": true\r\n\t}}\u0001{ \"type\": \"dv\", \"db\": \"moomin-dev-vladimir\", \"doc_id\": \"main_graph\" }\u0001{ \"type\": \"dfv\", \"db\": \"moomin-dev-vladimir\", \"doc_id\": \"main_graph\" }\u0001{ \"type\": \"df\", \"db\": \"moomin-dev-vladimir\", \"doc_id\": \"main_graph\" }\u0001{ \"type\": \"vz\", \"db\": \"moomin-dev-vladimir\", \"doc_id\": \"main_graph\" }\u0001{ \"type\": \"xc\", \"db\": \"moomin-dev-vladimir\", \"doc_id\": \"main_graph\" }\u0001{ \"type\": \"dv\", \"db\": \"moomin-dev-vladimir\", \"doc_id\": \"main_graph\" }\u0001{ \"type\": \"fdv\", \"db\": \"moomin-dev-vladimir\", \"doc_id\": \"main_graph\" }\u0001{ \"type\": \"sd\", \"db\": \"moomin-dev-vladimir\", \"doc_id\": \"main_graph\" }";
            var requestArray = tcpRequestSplitter.Split(textRequest);
        }
    }
}