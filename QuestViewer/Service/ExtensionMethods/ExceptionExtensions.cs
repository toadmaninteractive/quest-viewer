using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuestViewer
{
    public static class ExceptionExtensions
    {
        public static string ErrorReportText(this Exception ex)
        {
            return $"{ex.GetBaseException().Message}{Environment.NewLine}{ex.GetBaseException().StackTrace}";
        }
    }
}