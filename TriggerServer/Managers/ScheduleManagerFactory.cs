using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TriggerServer.Managers
{
    class ScheduleManagerFactory : IDisposable
    {
        Dictionary<Type, IScheduleManager> Managers { get; set; }

        public static ScheduleManagerFactory Instance { get; private set; }

        public ScheduleManagerFactory()
        {
            Instance = this;
        }

        public static IScheduleManager GetInstance(Type type)
        {
            return Instance.Managers[type];
        }

        public async Task Initialize(List<string> jobList)
        {
            var instances = typeof(IScheduleManager).Assembly.GetTypes()
                                              .Where(type => type.IsClass && type.GetInterfaces().Any(implementedInterface => typeof(IScheduleManager).Equals(implementedInterface)))
                                              .Select(type => (IScheduleManager)Activator.CreateInstance(type))
                                              .Where(manager => jobList.Any() == false || jobList.Contains(manager.GetJobName()));

            Managers = instances.ToDictionary(instance => instance.GetType(), instance => instance);
            await Task.WhenAll( Managers.Values.Select(instance => instance.Initialize()) );

        }

        public void Start()
        {
            foreach (var manager in Managers)
                manager.Value.StartScheduler();
            AkaLogger.Logger.Instance().Debug("Trigger Scheduler Start!");
        }

        public void Dispose()
        {
            foreach (var manager in Managers.Values)
                manager.Dispose();
        }
    }
}
