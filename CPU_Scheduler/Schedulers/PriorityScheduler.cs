using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CPU_Scheduler.Models;

namespace CPU_Scheduler.Schedulers
{
    internal class PriorityScheduler : BaseScheduler
    {
        private readonly bool _isPreemptive;

        public PriorityScheduler(bool isPreemptive = false)
        {
            _isPreemptive = isPreemptive;
        }

        public override string SchedulerName =>
            _isPreemptive ? "Priority (Preemptive)" : "Priority (Non-Preemptive)";

        public override Process? GetNextProcess(int currentTime)
        {
            // Non-preemptive: skip the queue scan entirely if current process is still running
            if (!_isPreemptive && CurrentProcess?.RemainingBurstTime > 0)
                return CurrentProcess;

            var next = ReadyQueue
                .Where(p => p.ArrivalTime <= currentTime && p.RemainingBurstTime > 0)
                .OrderBy(p => p.Priority)
                .ThenBy(p => p.ArrivalTime)
                .FirstOrDefault();

            if (next is null)
                return null;

            CurrentProcess = next;
            return CurrentProcess;
        }

        public override void Reset()
        {
            base.Reset();
            CurrentProcess = null;
        }
    }
}