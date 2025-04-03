using Json;
using NLog;
using System;
using System.Collections.Generic;

namespace QuestViewer.ServerForExternalClient
{
    public class ExternalClientRequestRegister
    {
        private Logger logger = LogManager.GetCurrentClassLogger();

        public class Result
        {
            public bool IsSuccess;
            public ExternalClientRequests.Request RequestObject;

            public Result(bool isSuccess, ExternalClientRequests.Request request)
            {
                IsSuccess = isSuccess;
                RequestObject = request;
            }
        }

        #region Request Name Mapper
        public readonly Dictionary<RequestTypeNames, string> RequestNameMapper = new Dictionary<RequestTypeNames, string>()
        {
            { RequestTypeNames.StatusCheck, ExternalClientRequests.StatusCheck.ExpectedTypeName },
            { RequestTypeNames.LevelStart, ExternalClientRequests.LevelStart.ExpectedTypeName },
            { RequestTypeNames.LevelFinish, ExternalClientRequests.LevelFinish.ExpectedTypeName },
            { RequestTypeNames.GraphState, ExternalClientRequests.GraphState.ExpectedTypeName },
            { RequestTypeNames.ShareState, ExternalClientRequests.ShareState.ExpectedTypeName }
        };

        public readonly Dictionary<string, RequestTypeNames> RequestNameInverseMapper = new Dictionary<string, RequestTypeNames>()
        {
            { ExternalClientRequests.StatusCheck.ExpectedTypeName, RequestTypeNames.StatusCheck },
            { ExternalClientRequests.LevelStart.ExpectedTypeName, RequestTypeNames.LevelStart },
            { ExternalClientRequests.LevelFinish.ExpectedTypeName, RequestTypeNames.LevelFinish},
            { ExternalClientRequests.GraphState.ExpectedTypeName, RequestTypeNames.GraphState},
            { ExternalClientRequests.ShareState.ExpectedTypeName, RequestTypeNames.ShareState }
        };

        public enum RequestTypeNames { StatusCheck, LevelStart, LevelFinish, GraphState, ShareState }
        #endregion

        public Result GetRequestObject(string source)
        {
            try
            {
                return GetRequestObjectInternal(source);
            }
            catch (System.Exception ex)
            {
                logger.Error(ex, $"TcpServer request exception: {ex.GetBaseException().Message}");
                return new Result(false, null);
            }
        }

        private Result GetRequestObjectInternal(string source)
        {
            var json = JsonParser.Parse(source);
            Result dataObject;
            if (json.AsObject.ContainsKey("type"))
            {
                var type = json.AsObject["type"].AsString;
                var requestTypeNames = RequestNameInverseMapper[type];

                switch (requestTypeNames)
                {
                    case RequestTypeNames.StatusCheck:
                        dataObject = new Result(true, new ExternalClientRequests.StatusCheck(json.AsObject));
                        break;
                    case RequestTypeNames.LevelStart:
                        dataObject = new Result(true, new ExternalClientRequests.LevelStart(json.AsObject));
                        break;
                    case RequestTypeNames.LevelFinish:
                        dataObject = new Result(true, new ExternalClientRequests.LevelFinish(json.AsObject));
                        break;
                    case RequestTypeNames.GraphState:
                        dataObject = new Result(true, new ExternalClientRequests.GraphState(json.AsObject));
                        break;
                    case RequestTypeNames.ShareState:
                        dataObject = new Result(true, new ExternalClientRequests.ShareState(json.AsObject));
                        break;
                    default:
                        logger.Warn($"RequestObject with 'type' {type} has not registration in {nameof(ExternalClientRequestRegister)} class", Environment.StackTrace);
                        dataObject = new Result(false, null);
                        break;
                }
            }
            else
            {
                logger.Warn("RequestObject has not property 'type'", Environment.StackTrace);
                dataObject = new Result(false, null);
            }
                
            return dataObject;
        }
    }
}