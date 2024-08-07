using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace WorkHoursManagementApp.Models
{
    public class WorkShiftData : INotifyPropertyChanged
    {
        private DateTime _startTime;
        private DateTime _endTime;
        private DateOnly _date;

        public DateTime StartTime
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

        public DateTime EndTime
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
            StartTime = DateTime.Today.AddHours(9);
            EndTime = DateTime.Today.AddHours(17);
        }

        public WorkShiftData(DateTime startTime, DateTime endTime, DateOnly date)
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