using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuestViewer
{
    public class UnicodeSymbolSplitter : ITcpRequestSplitter
    {
        public string DividerSymbol => "\u0001";

        public List<string> Split(string jsonText)
        {
            return jsonText.Split(DividerSymbol, StringSplitOptions.RemoveEmptyEntries).ToList();
        }
    }
}