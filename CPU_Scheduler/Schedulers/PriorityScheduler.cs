using CPU_Scheduler.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CPU_Scheduler.Schedulers
{
    public class PriorityScheduler : BaseScheduler
    {
        public override string SchedulerName => "Priority Scheduling";

        public override Process? GetNextProcess(int currentTime)
        {
            throw new NotImplementedException();
        }
    }
}