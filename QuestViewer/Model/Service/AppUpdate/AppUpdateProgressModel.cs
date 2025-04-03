using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuestViewer
{
    public class AppUpdateProgressModel
    {
        public int ProgressPercentage { get; set; }
        public long BytesReceived { get; set; }
        public long TotalBytesToReceive { get; set; }

        public AppUpdateProgressModel(int progressPercentage, long bytesReceived, long totalBytesToReceive)
        {
            ProgressPercentage = progressPercentage;
            BytesReceived = bytesReceived;
            TotalBytesToReceive = totalBytesToReceive;
        }

        public override string ToString()
        {
            return $"Downloading update: {(double)BytesReceived * 100 / TotalBytesToReceive:F2} %";
        }
    }
}