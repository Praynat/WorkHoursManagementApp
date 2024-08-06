using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace WorkHoursManagementApp.Models
{
    public class ExtraTimeData : INotifyPropertyChanged
    {
        private TimeSpan _extraTime;

        public TimeSpan ExtraTime
        {
            get => _extraTime;
            set
            {
                if (_extraTime != value)
                {
                    _extraTime = value;
                    OnPropertyChanged();
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public ExtraTimeData()
        {
            ExtraTime = TimeSpan.Zero;
        }

        public ExtraTimeData(TimeSpan extraTime)
        {
            ExtraTime = extraTime;
        }
    }
}

