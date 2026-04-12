using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CPU_Scheduler.Models;

namespace CPU_Scheduler.Schedulers
{
    internal class RoundRobinScheduler : BaseScheduler
    {
        //public int TimeQuantum { get; set; }
        private int _currentQuantumElapsed = 0;
        public override string SchedulerName => "Round Robin";

        public override Process? GetNextProcess(int currentTime)
        {
            // 1. If CPU is idle, grab the next process from the queue
            if (CurrentProcess == null)
            {
                if (ReadyQueue.Count == 0) return null;

                CurrentProcess = ReadyQueue[0];
                ReadyQueue.RemoveAt(0);

                if (CurrentProcess.StartTime == -1)
                    CurrentProcess.StartTime = currentTime;

                _currentQuantumElapsed = 0;
                CurrentProcess.State = "Running";
            }
            int TimeQuantum = CurrentProcess.TimeQuantum;
            // 2. We have an active process. Tick it forward by 1 second.
            CurrentProcess.RemainingBurstTime -= 1;
            _currentQuantumElapsed += 1;

            Process activeProcess = CurrentProcess;

            // 3. Has the process finished completely?
            if (CurrentProcess.RemainingBurstTime <= 0)
            {
                CurrentProcess.FinishTime = currentTime + 1; // It finishes at the end of this current tick
                CurrentProcess.RemainingBurstTime = 0;
                CurrentProcess.State = "Finished";

                // Clear the active slot so a new process gets loaded next tick
                CurrentProcess = null;
                _currentQuantumElapsed = 0;
            }
            // 4. Has the process reached the max time quantum?
            else if (_currentQuantumElapsed >= TimeQuantum)
            {
                CurrentProcess.State = "Waiting";
                CurrentProcess = null;
                _currentQuantumElapsed = 0;
            }

            return activeProcess;
        }
    }
}
/* PsuedoCode for main loop of all Schedulers
Function Process_Tick_Loop():
    // Step 1: Handle completely new arrivals FIRST
    For every Process in IncomingQueue:
        If Process.ArrivalTime == CurrentTime:
            Push Process to the BACK of the Scheduler_ReadyQueue
            Remove Process from IncomingQueue
    
    // Step 2: Handle preempted processes SECOND
    If there is a PreemptedProcess from the last tick:
        Push PreemptedProcess to the BACK of the Scheduler_ReadyQueue
        Clear the PreemptedProcess variable so it's empty
        
    // Step 3: Now that the queue is organized, simulate 1 second of work!
    ActiveProcess = Scheduler_RunNextProcess_ForOneSecond(CurrentTime)
    
    // Step 4: After doing the 1 second of work, check the limits!
    If ActiveProcess is completely finished:
        Mark it as "Finished"
    Else If ActiveProcess has reached the Time Quantum Limit:
        // We do NOT push it back to the queue yet! 
        // We just hold onto it in our pocket (the PreemptedProcess variable)
        // so that Step 2 can push it safely at the start of the next loop!
        PreemptedProcess = ActiveProcess
        
    // Step 5: Tick the clock forward!
    CurrentTime = CurrentTime + 1
*/