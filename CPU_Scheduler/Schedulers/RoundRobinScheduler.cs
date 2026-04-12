using CPU_Scheduler.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CPU_Scheduler.Schedulers
{
    internal class RoundRobinScheduler : IScheduler
    {
        string IScheduler.SchedulerName => throw new NotImplementedException();

        void IScheduler.AddProcess(Process process)
        {
            throw new NotImplementedException();
        }

        Process? IScheduler.GetNextProcess(int currentTime)
        {
            throw new NotImplementedException();
        }

        bool IScheduler.HasProcesses()
        {
            throw new NotImplementedException();
        }

        void IScheduler.Reset()
        {
            throw new NotImplementedException();
        }
    }
}
