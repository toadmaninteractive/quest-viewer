using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuestGraph.Core
{
    public class ExceptionConstructor
    {
        public static InvalidOperationException UnexpectedEnumValue<T>(T enumValue, string stackTrace)
        {
            return new InvalidOperationException($"{string.Format(Texts.CustomExceptionErrors.UnexpectedEnumValue, enumValue, nameof(T))}{Environment.NewLine}{stackTrace}");
        }
    }
}