using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuestViewer.ServerForExternalClient
{
    public interface IServerForExternalClient
    {
        event Action<int> OnActiveConnectionCountChanged;

        void Start();
        Task StartListennig();
        void Stop();
    }
}