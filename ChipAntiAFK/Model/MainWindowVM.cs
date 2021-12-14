using System.Diagnostics;
using System.Windows.Input;

namespace ChipAntiAFK
{
    public class MainWindowVM : NotifyPropertyChangedBase
    {
        public MainWindowVM()
        {
            // initial values
            StartStopButtonText = "Start";
            ProcessInfoText = "Click 'Open Process'.";
            TotalTimeRunningText = "Select FFXIV.";
            NextActionInText = "";

            Program.Instance.TimerUpdatedEvent += (obj, args) =>
            {
                TotalTimeRunningText = string.Format(
                    "Time AFK: {0:00}:{1:00}:{2:00}:{3:00}",
                    args.RunTime.Hours, args.RunTime.Minutes, args.RunTime.Seconds, args.RunTime.Milliseconds / 10
                );
                NextActionInText = string.Format(
                    "Next action in: {0:00}:{1:00}:{2:00}",
                    args.ActTime.Minutes, args.ActTime.Seconds, args.ActTime.Milliseconds / 10
                );
            };

            Program.Instance.PropertyChanged += (o, e) =>
            {
                if (e.PropertyName.Equals(nameof(Program.Instance.ActiveProcess)))
                {
                    ProcessInfoText = string.Format("{0} ({1})", Program.Instance.ActiveProcess.ProcessName, Program.Instance.ActiveProcess.Id);
                    TotalTimeRunningText = "";
                    NextActionInText = "Click 'Start'.";
                }

                if (e.PropertyName.Equals(nameof(Program.Instance.IsRunning)))
                {
                    StartStopButtonText = IsRunning ? "Stop" : "Start";
                }
            };
        }

        public bool IsRunning => Program.Instance.IsRunning;

        public string StartStopButtonText
        {
            get => _startStopButtonText;
            private set => SetValue(ref _startStopButtonText, value);
        }

        public string ProcessInfoText
        {
            get => _processInfoText;
            private set => SetValue(nameof(ProcessInfoText), ref _processInfoText, value);
        }

        public string TotalTimeRunningText
        {
            get => _totalTimeRunningText;
            private set => SetValue(nameof(TotalTimeRunningText), ref _totalTimeRunningText, value);
        }

        public string NextActionInText
        {
            get => _nextActionInText;
            private set => SetValue(nameof(NextActionInText), ref _nextActionInText, value);
        }

        public ICommand OpenProcessButtonCommand
        {
            get
            {
                return new RelayCommand(
                    (o) =>
                    {
                        return Program.Instance.ActiveProcess == null;
                    },
                    (o) =>
                    {
                        // this kind of breaks MVVM but i don't care
                        ProcessSelection procSelect = new ProcessSelection();
                        procSelect.Show();
                    }
                );
            }
        }

        public ICommand StartStopButtonCommand
        {
            get
            {
                return new RelayCommand(
                    (o) =>
                    {
                        return Program.Instance.ActiveProcess != null;
                    },
                    (o) =>
                    {
                        Program.Instance.StartStop();
                    }
                );
            }
        }

        public ICommand DonateButtonCommand => new RelayCommand(p => true, p => Process.Start("explorer", "https://ko-fi.com/trotlinebeercan"));

        private string _startStopButtonText;
        private string _processInfoText;
        private string _totalTimeRunningText;
        private string _nextActionInText;
    }
}
