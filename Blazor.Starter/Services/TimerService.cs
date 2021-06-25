using System;
using System.Timers;

namespace Blazor.Starter.Services
{
    public class TimerService
    {
        public double Interval { get; set; } = 6000;
        private Timer _timer;

        public event EventHandler<TimerEventArgs> TimerCompleted;

        public bool StartTimer(double interval = 0, bool autoreset = false)
        {
            if (!_timer.Enabled)
            {
                this.Interval = interval > 0 ? interval : this.Interval; 
                _timer = new Timer(interval);
                _timer.Elapsed += OnCounterElapsed;
                _timer.Enabled = true;
                _timer.AutoReset = autoreset;
                _timer.Start();
                return true;
            }
            else 
                return false;
        }

        public bool ResetTimer()
        {
            _timer.Enabled = false;
            return !_timer.Enabled;
        }

        private void OnCounterElapsed(object sender, ElapsedEventArgs e)
        {
            _timer.Stop();
            TimerCompleted?.Invoke(this, new TimerEventArgs { ElapsedInterval = _timer.Interval, EventTimeStamp = e.SignalTime });
        }
    }

    public class TimerEventArgs : EventArgs
    {
        public double ElapsedInterval { get; set; }
        public DateTime EventTimeStamp { get; set; }
    }
}