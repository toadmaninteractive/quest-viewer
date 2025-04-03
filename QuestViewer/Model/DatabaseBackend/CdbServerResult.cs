using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuestViewer
{
    public class CdbServerResult<T> where T : class
    {
        public bool IsSuccessStatusCode;
        public T Value;

        public CdbServerResult(T value, bool isSuccessStatusCode)
        {
            Value = value;
            IsSuccessStatusCode = isSuccessStatusCode;
        }
    }
}