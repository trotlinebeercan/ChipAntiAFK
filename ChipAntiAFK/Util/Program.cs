using System;
using System.Diagnostics;
using System.Threading;

namespace ChipAntiAFK
{
    public class Program : NotifyPropertyChangedBase
    {
        #region Constructor and Singleton impl
        public static Program Instance { get { return __arbitur__.Value; } }
        private static readonly Lazy<Program> __arbitur__ = new Lazy<Program>(() => new Program());
        private Program() { }
        #endregion
        
        public class TimerUpdatedEventArgs
        {
            public TimeSpan RunTime;
            public TimeSpan ActTime;
        }

        public EventHandler<TimerUpdatedEventArgs> TimerUpdatedEvent;
        
        public void StartStop()
        {
            if (IsRunning) Stop(); else Start();
        }

        private void Start()
        {
            Event = new AutoResetEvent(false);
            RunTime  = new TimeSpan(0);
            ActTime  = new TimeSpan(0);
            RunTimer = new Timer(ExecutionThread, _autoEvent, 0, TimePeriodInMs);

            IsRunning = true;
        }

        private void Stop()
        {
            RunTime = new TimeSpan(0);
            ActTime = new TimeSpan(0);

            IsRunning = false;

            _autoEvent.WaitOne();

            TimerUpdatedEvent?.Invoke(this, new TimerUpdatedEventArgs
            {
                RunTime = RunTime,
                ActTime = ActTime
            });
        }

        private void ExecutionThread(object autoEvent)
        {
            if (!IsRunning)
            {
                (autoEvent as AutoResetEvent).Set();
                return;
            }

            RunTime += new TimeSpan(0, 0, 0, 0, TimePeriodInMs);
            ActTime -= new TimeSpan(0, 0, 0, 0, TimePeriodInMs);

            if (ActTime.Equals(TimeSpan.Zero) || ActTime.Ticks < 0)
            {
                KeyCommand.SendJump(ActiveProcess);
                ActTime = new TimeSpan(0, 0, 0, 0, RandomNumber.Random(MinWaitTimeInMs, MaxWaitTimeInMs));
            }

            TimerUpdatedEvent?.Invoke(this, new TimerUpdatedEventArgs
            {
                RunTime = RunTime,
                ActTime = ActTime
            });
        }

        public bool IsRunning
        {
            get => _isRunning;
            private set => SetValue(ref _isRunning, value);
        }

        public AutoResetEvent Event
        {
            get => _autoEvent;
            private set => SetValue(ref _autoEvent, value);
        }

        public Process ActiveProcess
        {
            get => _process;
            set => SetValue(ref _process, value);
        }

        private int MinWaitTimeInMs => 60000;
        private int MaxWaitTimeInMs => 300000;
        private int TimePeriodInMs => 500;

        private bool _isRunning;
        private AutoResetEvent _autoEvent;
        private Process _process;

        private Timer RunTimer;
        private TimeSpan RunTime;
        private TimeSpan ActTime;
    }
}
