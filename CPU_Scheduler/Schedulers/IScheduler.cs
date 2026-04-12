using CPU_Scheduler.Models;

namespace CPU_Scheduler.Schedulers
{
    public interface IScheduler 
    {
        string SchedulerName { get; }
        void AddProcess(Process process);
        Process? GetNextProcess(int currentTime);
        bool HasProcesses();
        void Reset();
    }
}