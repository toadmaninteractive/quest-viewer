using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuestViewer
{
    public interface ITcpRequestSplitter
    {
        string DividerSymbol {  get; }
        List<string> Split(string jsonText);
    }
}