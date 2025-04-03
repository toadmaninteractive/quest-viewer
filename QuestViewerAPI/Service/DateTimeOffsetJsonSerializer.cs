using Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuestGraph.Core
{
    public class DateTimeOffsetJsonSerializer
    {
        public DateTimeOffset Deserialize(ImmutableJson json) => DateTimeOffset.Parse(json.AsString);

        public ImmutableJson Serialize(DateTimeOffset value) => value.ToString("yyyy-MM-ddTHH\\:mm\\:ss.fff");

        public static readonly DateTimeOffsetJsonSerializer Instance = new();
    }
}