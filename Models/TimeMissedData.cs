using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace WorkHoursManagementApp.Models
{
    public class TimeMissedData : INotifyPropertyChanged
    {
        private TimeSpan _timeMissed;

        public TimeSpan TimeMissed
        {
            get => _timeMissed;
            set
            {
                if (_timeMissed != value)
                {
                    _timeMissed = value;
                    OnPropertyChanged();
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public TimeMissedData()
        {
            TimeMissed = TimeSpan.Zero;
        }

        public TimeMissedData(TimeSpan timeMissed)
        {
            TimeMissed = timeMissed;
        }
    }

}
