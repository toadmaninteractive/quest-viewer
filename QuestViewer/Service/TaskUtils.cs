using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace QuestViewer
{
    public static class TaskUtils
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        public static void Track(this Task task)
        {
            task.ContinueWith(HandleException);
        }

        public static void HandleException(Task task)
        {
            if (task.Exception != null)
            {
                logger.Error(InnerException(task.Exception));
            }
        }

        public static Exception InnerException(Exception exception)
        {
            if (exception is AggregateException)
                return ((AggregateException)exception).InnerException;
            if (exception is WebException && ((WebException)exception).InnerException != null)
                return ((WebException)exception).InnerException;
            else
                return exception;
        }

        public static string ExceptionMessage(Exception exception)
        {
            if (exception is AggregateException)
                return ((AggregateException)exception).InnerException.Message;
            else
                return exception.Message;
        }
    }
}