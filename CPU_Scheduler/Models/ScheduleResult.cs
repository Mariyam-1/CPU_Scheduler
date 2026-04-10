using System.Collections.Generic;
using System.Linq;

namespace CPU_Scheduler.Models
{
    public class GanttEntry
    {
        public string ProcessID { get; set; } = string.Empty;
        public int StartTime { get; set; }
        public int EndTime { get; set; }
        public int Duration => EndTime - StartTime;

        public override string ToString() => $"[{ProcessID}] {StartTime}→{EndTime}";
    }

    public class ScheduleResult
    {
        public List<GanttEntry> GanttChart { get; set; } = new();
        public List<Process> CompletedProcesses { get; set; } = new();

        public double AverageWaitingTime
            => CompletedProcesses.Count == 0
               ? 0
               : CompletedProcesses.Average(p => p.WaitingTime);

        public double AverageTurnaroundTime
            => CompletedProcesses.Count == 0
               ? 0
               : CompletedProcesses.Average(p => p.TurnaroundTime);

        public void AppendGanttTick(string processID, int currentTick)
        {
            if (GanttChart.Count > 0)
            {
                var last = GanttChart[^1];
                if (last.ProcessID == processID)
                {
                    last.EndTime = currentTick + 1;
                    return;
                }
            }

            GanttChart.Add(new GanttEntry
            {
                ProcessID = processID,
                StartTime = currentTick,
                EndTime = currentTick + 1
            });
        }

        public void MarkCompleted(Process process, int finishTime)
        {
            process.FinishTime = finishTime;
            process.State = "Finished";
            if (!CompletedProcesses.Contains(process))
                CompletedProcesses.Add(process);
        }

        public void Reset()
        {
            GanttChart.Clear();
            CompletedProcesses.Clear();
        }
    }
}