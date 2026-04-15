using CPU_Scheduler.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CPU_Scheduler.Schedulers
{
    // Inherit from BaseScheduler to get the ReadyQueue and HasProcesses logic
    public class FCFSScheduler : BaseScheduler
    {
        public override string SchedulerName => "First Come First Served";

        public override Process? GetNextProcess(int currentTime)
        {
            if (CurrentProcess != null && CurrentProcess.RemainingBurstTime > 0)
                return CurrentProcess;
            CurrentProcess = ReadyQueue.Where(p => p.ArrivalTime <= currentTime && p.RemainingBurstTime > 0).
                OrderBy(p => p.ArrivalTime).ThenBy(p => p.BurstTime).FirstOrDefault();
            return CurrentProcess;
        }
    }
}