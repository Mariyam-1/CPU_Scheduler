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
            return CurrentProcess;
        }
    }
}
