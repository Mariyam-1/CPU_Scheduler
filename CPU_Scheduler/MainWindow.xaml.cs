using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;
using CPU_Scheduler.Models;
using CPU_Scheduler.Schedulers;

namespace CPU_Scheduler
{
    public partial class MainWindow : Window
    {
        public ObservableCollection<Process> Processes { get; set; } = new();
        private ScheduleResult _result = new();
        private DispatcherTimer _timer;
        private int _currentTime = 0;

        // This would be initialized based on your ComboBox selection
        public string selectedAlgo;
        private IScheduler _currentScheduler;
        ScheduleResult res;


        public MainWindow()
        {
            InitializeComponent();
            ProcessGrid.ItemsSource = Processes;

            _timer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(1) };
            _timer.Tick += Timer_Tick;
        }

        private void AddProcess_Click(object sender, RoutedEventArgs e)
        {
            if (int.TryParse(InputBurst.Text, out int burst))
            {
                int.TryParse(InputPriority.Text, out int priority);

                // 1. Create the process
                var p = new Process(InputPID.Text, _currentTime, burst, priority);

                // 2. Add to UI collection (updates the DataGrid)
                Processes.Add(p);

                // 3. FIXED: If the scheduler is already running, add it to the logic queue immediately
                if (_currentScheduler != null)
                {
                    _currentScheduler.AddProcess(p);
                }

                InputPID.Clear();
                InputBurst.Clear();
                InputPriority.Clear();
            }
        }

        private void StartBtn_Click(object sender, RoutedEventArgs e)
        {
            if (Processes.Count == 0) return;

            _currentTime = 0;
            _result.Reset();
            GanttContainer.Children.Clear();
    
            selectedAlgo = (AlgoSelector.SelectedItem as ComboBoxItem).Content.ToString();
            _currentScheduler = selectedAlgo switch
            {
                "FCFS" => new FCFSScheduler(),
                "SJF (Non-Preemptive)" => new SJFScheduler(false),
                "SJF (Preemptive)" => new SJFScheduler(true),
                "Priority" => new PriorityScheduler()
            };

            foreach (var p in Processes)
            {
                p.Reset();
                _currentScheduler.AddProcess(p);
            }


            _timer.Start();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            Process current = _currentScheduler.GetNextProcess(_currentTime);

            if (current != null)
            {
                _result.AppendGanttTick(current.ProcessID, _currentTime);
        
                if (current.StartTime == -1) current.StartTime = _currentTime;
                current.RemainingBurstTime--;
                _currentTime++;

                if (current.RemainingBurstTime == 0)
                {
                    _result.MarkCompleted(current, _currentTime);
                }

                TimeLabel.Text = $"Time: {_currentTime}s";
                RefreshGanttUI();
            }
            else
            {
                if (!_currentScheduler.HasProcesses())
                {
                    _timer.Stop();
                    MessageBox.Show("Finished!");
                }
                else
                {
                    _currentTime++;
                }
            }
        }
        private void RefreshGanttUI()
        {
            GanttContainer.Children.Clear();
            foreach (var entry in _result.GanttChart)
            {
                var originalProcess = Processes.FirstOrDefault(p => p.ProcessID == entry.ProcessID);
                var processBrush = originalProcess?.ProcessColor ?? Brushes.DodgerBlue;

                var block = new Border
                {
                    Width = entry.Duration * 40,
                    Background = processBrush,
                    BorderBrush = Brushes.White,
                    BorderThickness = new Thickness(1),
                    Child = new TextBlock
                    {
                        Text = entry.ProcessID,
                        Foreground = Brushes.White,
                        FontWeight = FontWeights.Bold,
                        HorizontalAlignment = HorizontalAlignment.Center,
                        VerticalAlignment = VerticalAlignment.Center
                    }
                };
                GanttContainer.Children.Add(block);
            }
        }

        private void AlgoSelector_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}