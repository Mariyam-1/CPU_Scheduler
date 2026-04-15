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
        private bool _isSimulationActive = false;
        private bool _hasStarted = false;
        private string _currentMode = "Dynamic";

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

            QuantumPanel.Visibility = Visibility.Collapsed;
            PriorityPanel.Visibility = Visibility.Collapsed;
            PreemptionPanel.Visibility = Visibility.Collapsed;

        }

        private void AddProcess_Click(object sender, RoutedEventArgs e)
        {
            if (_hasStarted && _currentMode == "Static")
            {
                MessageBox.Show("Cannot add processes while a Static simulation is running.", "Warning");
                return;
            }

            if (_isSimulationActive && _currentMode == "Dynamic")
            {
                MessageBox.Show("Please Pause the simulation before adding a dynamic process.", "Warning");
                return;
            }

            if (int.TryParse(InputBurst.Text, out int burst))
            {
                int.TryParse(InputPriority.Text, out int priority);

                int arrival = _currentTime;
                if (_currentMode == "Static")
                {
                    if (!int.TryParse(InputArrival.Text, out arrival)) arrival = 0;
                }

                var p = new Process(InputPID.Text, arrival, burst, priority);
                Processes.Add(p);

                if (_hasStarted && _currentScheduler != null)
                {
                    _currentScheduler.AddProcess(p);
                }

                InputPID.Clear();
                InputBurst.Clear();
                InputPriority.Clear();
            }
        }

        private void ClearProcess_Click(object sender, RoutedEventArgs e)
        {
            if (_isSimulationActive)
            {
                MessageBox.Show("Please pause the simulation before clearing processes.", "Warning");
                return;
            }

            // Halt simulation entirely
            ResetBtn_Click(null, null);

            // Nuke the source data
            Processes.Clear();
            ProcessGrid.Items.Refresh();
        }

        private void StartBtn_Click(object sender, RoutedEventArgs e)
        {
            if (_currentMode == "Static")
            {
                if (_timer != null) _timer.Interval = TimeSpan.FromMilliseconds(0);
            }
            else
            {
                if (_timer != null) _timer.Interval = TimeSpan.FromSeconds(1);
            }
            if (Processes.Count == 0) return;

            if (_isSimulationActive)
            {
                _timer.Stop();
                _isSimulationActive = false;
                StartBtn.Content = "▶ RESUME";
                StartBtn.Background = Brushes.Orange;
                return;
            }

            if (!_hasStarted)
            {
                _currentTime = 0;
                _result.Reset();
                GanttContainer.Children.Clear();

                selectedAlgo = (AlgoSelector.SelectedItem as ComboBoxItem).Content.ToString();
                int.TryParse(InputQuantum.Text, out int quantum);
                if (quantum <= 0) quantum = 2;

                string preemptionType = (PreemptionSelector.SelectedItem as ComboBoxItem)?.Content?.ToString();
                bool isPreemptive = preemptionType == "Preemptive";

                _currentScheduler = selectedAlgo switch
                {
                    "FCFS" => new FCFSScheduler(),
                    "SJF" => new SJFScheduler(isPreemptive),
                    "Priority" => new PriorityScheduler(isPreemptive),
                    "Round Robin" => new RoundRobinScheduler(quantum),
                    _ => new FCFSScheduler()
                };

                foreach (var p in Processes)
                {
                    p.Reset();
                    _currentScheduler.AddProcess(p);
                }

                _hasStarted = true;
                ResetBtn.Visibility = Visibility.Visible;
            }

            _timer.Start();
            _isSimulationActive = true;
            StartBtn.Content = "⏸ PAUSE";
            StartBtn.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#EF4444"));
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

                var grid = new Grid();

                grid.Children.Add(new TextBlock
                {
                    Text = entry.ProcessID,
                    Foreground = Brushes.White,
                    FontWeight = FontWeights.Bold,
                    FontSize = 18,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center
                });

                grid.Children.Add(new TextBlock
                {
                    Text = entry.StartTime.ToString(),
                    Foreground = Brushes.White,
                    FontSize = 10,
                    Margin = new Thickness(4, 0, 0, 2),
                    HorizontalAlignment = HorizontalAlignment.Left,
                    VerticalAlignment = VerticalAlignment.Bottom
                });

                grid.Children.Add(new TextBlock
                {
                    Text = entry.EndTime.ToString(),
                    Foreground = Brushes.White,
                    FontSize = 10,
                    Margin = new Thickness(0, 0, 4, 2),
                    HorizontalAlignment = HorizontalAlignment.Right,
                    VerticalAlignment = VerticalAlignment.Bottom
                });

                var block = new Border
                {
                    Width = entry.Duration * 80,
                    Height = 120,
                    Background = processBrush,
                    BorderBrush = Brushes.White,
                    BorderThickness = new Thickness(2),
                    CornerRadius = new CornerRadius(4),
                    Margin = new Thickness(0, 0, 4, 0),
                    Child = grid
                };
                GanttContainer.Children.Add(block);
            }

            AvgWaitText.Text = $"Avg Wait: {_result.AverageWaitingTime:0.0}s";
            AvgTurnText.Text = $"Avg Turnaround: {_result.AverageTurnaroundTime:0.0}s";
            ProcessGrid.Items.Refresh();
        }

        private void ResetBtn_Click(object sender, RoutedEventArgs e)
        {
            _timer.Stop();
            _isSimulationActive = false;
            _hasStarted = false;
            _currentTime = 0;
            TimeLabel.Text = $"Time: 0s";
            _result.Reset();
            GanttContainer.Children.Clear();
            foreach (var p in Processes) p.Reset();

            StartBtn.Content = "▶ RUN ALGORITHM";
            StartBtn.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#10B981"));
            ResetBtn.Visibility = Visibility.Collapsed;
            ProcessGrid.Items.Refresh();
        }

        private void ModeSelector_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ArrivalPanel == null) return;
            var selected = (ModeSelector.SelectedItem as ComboBoxItem)?.Content?.ToString();
            _currentMode = selected ?? "Dynamic";

            if (_currentMode == "Static")
                ArrivalPanel.Visibility = Visibility.Visible;
            else
                ArrivalPanel.Visibility = Visibility.Collapsed;
        }



        private void AlgoSelector_SelectionChanged_1(object sender, SelectionChangedEventArgs e)
        {
            if (AlgoSelector == null || PriorityPanel == null || QuantumPanel == null || PreemptionPanel == null) return;

            var selected = (AlgoSelector.SelectedItem as ComboBoxItem)?.Content?.ToString();

            if (selected == "Round Robin")
            {
                QuantumPanel.Visibility = Visibility.Visible;
                PriorityPanel.Visibility = Visibility.Collapsed;
                PreemptionPanel.Visibility = Visibility.Collapsed;
            }
            else if (selected == "Priority")
            {
                QuantumPanel.Visibility = Visibility.Collapsed;
                PriorityPanel.Visibility = Visibility.Visible;
                PreemptionPanel.Visibility = Visibility.Visible;
            }
            else if (selected == "SJF")
            {
                QuantumPanel.Visibility = Visibility.Collapsed;
                PriorityPanel.Visibility = Visibility.Collapsed;
                PreemptionPanel.Visibility = Visibility.Visible;
            }
            else
            {
                QuantumPanel.Visibility = Visibility.Collapsed;
                PriorityPanel.Visibility = Visibility.Collapsed;
                PreemptionPanel.Visibility = Visibility.Collapsed;
            }
        }
    }
}