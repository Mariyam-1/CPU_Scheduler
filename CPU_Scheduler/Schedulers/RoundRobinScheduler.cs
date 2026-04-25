using System.Collections.Generic;
using System.Linq;
using CPU_Scheduler.Models;

namespace CPU_Scheduler.Schedulers
{
    public class RoundRobinScheduler : BaseScheduler
    {
        public int TimeQuantum { get; set; }
        public override string SchedulerName => "Round Robin";

        private Queue<Process> _activeQueue = new();
        private int _quantumElapsed = 0;
        private HashSet<Process> _arrivedProcesses = new();

        public RoundRobinScheduler(int quantum) => TimeQuantum = quantum;

        public override Process? GetNextProcess(int currentTime)
        {
            var newlyArrived = ReadyQueue
                .Where(p => p.ArrivalTime <= currentTime && !_arrivedProcesses.Contains(p))
                .OrderBy(p => p.ArrivalTime)
                .ToList();

            foreach (var p in newlyArrived)
            {
                _arrivedProcesses.Add(p);
                _activeQueue.Enqueue(p);
            }

            if (CurrentProcess != null)
            {
                if (CurrentProcess.RemainingBurstTime <= 0)
                {
                    CurrentProcess = null;
                    _quantumElapsed = 0;
                }
                else if (_quantumElapsed >= TimeQuantum)
                {
                    CurrentProcess.State = "Waiting";
                    _activeQueue.Enqueue(CurrentProcess);
                    CurrentProcess = null;
                    _quantumElapsed = 0;
                }
            }

            if (CurrentProcess == null)
            {
                if (_activeQueue.Count > 0)
                {
                    CurrentProcess = _activeQueue.Dequeue();
                    CurrentProcess.State = "Running";
                    _quantumElapsed = 0;
                }
            }

            if (CurrentProcess != null)
            {
                _quantumElapsed++;
            }

            return CurrentProcess;
        }

        public override void Reset()
        {
            base.Reset();
            _activeQueue.Clear();
            _arrivedProcesses.Clear();
            _quantumElapsed = 0;
        }
    }
}