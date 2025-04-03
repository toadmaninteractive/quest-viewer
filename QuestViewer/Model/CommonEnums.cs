using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuestViewer
{
    public enum UserNotificationType
    {
        Error, Warning, Trace
    }

    public enum ApplicationUpdateChannel
    {
        Stable,
        Beta,
        Dev
    }
}