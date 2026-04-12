using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Media;

namespace CPU_Scheduler.Models
{
    public class Process : INotifyPropertyChanged
    {
        private int _remainingBurstTime;
        private string _state = "Waiting";

        public string ProcessID { get; set; } = string.Empty;
        public int ArrivalTime { get; set; }
        public int BurstTime { get; set; }
        public int Priority { get; set; }
        public int TimeQuantum { get; set; }
        public Brush ProcessColor { get; set; }

        public int RemainingBurstTime
        {
            get => _remainingBurstTime;
            set
            {
                if (_remainingBurstTime == value) return;
                _remainingBurstTime = value;
                OnPropertyChanged();
            }
        }

        public string State
        {
            get => _state;
            set
            {
                if (_state == value) return;
                _state = value;
                OnPropertyChanged();
            }
        }

        public int StartTime { get; set; } = -1;
        public int FinishTime { get; set; }

        public int TurnaroundTime => FinishTime - ArrivalTime;
        public int WaitingTime => TurnaroundTime - BurstTime;

        public Process(string processID, int arrivalTime, int burstTime,
                       int priority = 0, int timeQuantum = 0)
        {
            ProcessID = processID;
            ArrivalTime = arrivalTime;
            BurstTime = burstTime;
            Priority = priority;
            TimeQuantum = timeQuantum;
            RemainingBurstTime = burstTime;

            Random rnd = new Random();
            byte[] rgb = new byte[3];
            rnd.NextBytes(rgb);
            ProcessColor = new SolidColorBrush(Color.FromRgb(rgb[0], rgb[1], rgb[2]));
        }

        public void Reset()
        {
            RemainingBurstTime = BurstTime;
            StartTime = -1;
            FinishTime = 0;
            State = "Waiting";
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string? name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

        public override string ToString() => ProcessID;
    }
}