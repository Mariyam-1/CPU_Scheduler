using System.Collections.Generic;
using System.Linq;
using CPU_Scheduler.Models;

namespace CPU_Scheduler.Schedulers
{
    public abstract class BaseScheduler : IScheduler
    {
        protected List<Process> ReadyQueue = new();
        protected Process? CurrentProcess = null;

        public abstract string SchedulerName { get; }
        public abstract Process? GetNextProcess(int currentTime);

        public virtual void AddProcess(Process process)
        {
            process.State = "Waiting";
            ReadyQueue.Add(process);
        }

        public bool HasProcesses()
        {
            return ReadyQueue.Any(p => p.RemainingBurstTime > 0);
        }

        public virtual void Reset()
        {
            ReadyQueue.Clear();
            CurrentProcess = null;
        }
    }
}