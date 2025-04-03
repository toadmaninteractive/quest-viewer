using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuestViewer
{
    public class EmptySymbolSplitter : ITcpRequestSplitter
    {
        public string DividerSymbol => string.Empty;

        public List<string> Split(string jsonText)
        {
            var array = jsonText.Trim('{').Trim('}').Split("}{", StringSplitOptions.RemoveEmptyEntries);
            return array.Select(x => $"{{{x}}}").ToList();
        }
    }
}