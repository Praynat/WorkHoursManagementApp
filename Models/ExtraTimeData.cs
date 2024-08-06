using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace WorkHoursManagementApp.Models
{
    public class ExtraTimeData : INotifyPropertyChanged
    {
        private DateTime _extraTime;

        public DateTime ExtraTime
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
            ExtraTime = DateTime.Today;
        }

        public ExtraTimeData(DateTime extraTime)
        {
            ExtraTime = extraTime;
        }

        public TimeSpan GetExtraTimeDuration()
        {
            return ExtraTime.TimeOfDay;
        }
    }
}
