using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace WorkHoursManagementApp.Models
{
    public class WorkShiftData : INotifyPropertyChanged
    {
        private TimeSpan _startTime;
        private TimeSpan _endTime;
        private DateOnly _date;

        public TimeSpan StartTime
        {
            get => _startTime;
            set
            {
                if (_startTime != value)
                {
                    _startTime = value;
                    OnPropertyChanged();
                }
            }
        }

        public TimeSpan EndTime
        {
            get => _endTime;
            set
            {
                if (_endTime != value)
                {
                    _endTime = value;
                    OnPropertyChanged();
                }
            }
        }

        public DateOnly Date
        {
            get => _date;
            set
            {
                if (_date != value)
                {
                    _date = value;
                    OnPropertyChanged();
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public WorkShiftData()
        {
            Date = DateOnly.FromDateTime(DateTime.Today);
            StartTime = new TimeSpan(9, 0, 0);
            EndTime = new TimeSpan(17, 0, 0);
        }

        public WorkShiftData(TimeSpan startTime, TimeSpan endTime, DateOnly date)
        {
            StartTime = startTime;
            EndTime = endTime;
            Date = date;
        }

        public TimeSpan GetWorkShiftDuration()
        {
            return EndTime - StartTime;
        }
    }
}
