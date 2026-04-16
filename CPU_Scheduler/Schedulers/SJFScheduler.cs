using CPU_Scheduler.Schedulers;
using CPU_Scheduler.Models;

namespace CPU_Scheduler.Schedulers
{
    public class SJFScheduler : BaseScheduler
    {
        public override string SchedulerName => "Shortest Job First";
        public bool IsPreemptive { get; set; }

        public SJFScheduler(bool isPreemptive = false)
        {
            IsPreemptive = isPreemptive;
        }
        public override Process? GetNextProcess(int currentTime)
        {
            if (CurrentProcess != null && CurrentProcess.RemainingBurstTime > 0 && !IsPreemptive)
                return CurrentProcess;
            CurrentProcess = ReadyQueue.Where(p => p.ArrivalTime <= currentTime && p.RemainingBurstTime > 0).
                OrderBy(p => p.RemainingBurstTime).ThenBy(p => p.ArrivalTime).FirstOrDefault();
            return CurrentProcess;
        }
    }
}