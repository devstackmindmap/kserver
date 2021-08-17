using System;
using System.Threading.Tasks;
using CommonProtocol;

namespace TriggerServer.Managers
{
    interface IScheduleManager : IDisposable
    {
        Task Initialize();

        void StartScheduler();

        void DoCommand(MessageType msgType, BaseProtocol requestInfo);

        string GetJobName();
    }
}
