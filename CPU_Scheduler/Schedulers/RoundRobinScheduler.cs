using CPU_Scheduler.Models;

namespace CPU_Scheduler.Schedulers
{
    public class RoundRobinScheduler : BaseScheduler
    {
        public int TimeQuantum { get; set; }
        public override string SchedulerName => "Round Robin";
        private int _index = -1;
        private int _quantumElapsed = 0;

        public RoundRobinScheduler(int quantum) => TimeQuantum = quantum;

        public override void AddProcess(Process process)
        {
            process.State = "Waiting";
            int insertPos = _index + 1;
            ReadyQueue.Insert(insertPos, process);
            _index = insertPos;
        }

        public override Process? GetNextProcess(int currentTime)
        {
            if (CurrentProcess != null && CurrentProcess.RemainingBurstTime <= 0)
            {
                CurrentProcess = null;
                _quantumElapsed = 0;
            }

            if (CurrentProcess == null)
            {
                CurrentProcess = AdvanceIndex();
                _quantumElapsed = 0;
                if (CurrentProcess == null) return null;

                CurrentProcess.State = "Running";
            }

            _quantumElapsed++;
            Process active = CurrentProcess;

            if (_quantumElapsed >= TimeQuantum)
            {
                CurrentProcess.State = "Waiting";
                CurrentProcess = null;
            }

            return active;
        }

        public override void Reset()
        {
            base.Reset();
            _index = -1;
            _quantumElapsed = 0;
        }

        private Process? AdvanceIndex()
        {
            if (!HasProcesses()) return null;

            for (int step = 1; step <= ReadyQueue.Count; step++)
            {
                int idx = (_index + step) % ReadyQueue.Count;
                if (ReadyQueue[idx].RemainingBurstTime > 0)
                {
                    _index = idx;
                    return ReadyQueue[idx];
                }
            }
            return null;
        }
    }
}