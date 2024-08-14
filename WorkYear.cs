using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace WorkHoursManagementApp
{
    public class WorkYear : INotifyPropertyChanged
    {
        private DateTime _workYearStartDate;
        private DateTime _workYearEndDate;
        private string _workYearName;

        public DateTime WorkYearStartDate
        {
            get => _workYearStartDate;
            set
            {
                if (_workYearStartDate != value)
                {
                    _workYearStartDate = value;
                    OnPropertyChanged(nameof(WorkYearStartDate));
                }
            }
        }

        public DateTime WorkYearEndDate
        {
            get => _workYearEndDate;
            set
            {
                if (_workYearEndDate != value)
                {
                    _workYearEndDate = value;
                    OnPropertyChanged(nameof(WorkYearEndDate));
                }
            }
        }

        public string WorkYearName
        {
            get => _workYearName;
            set
            {
                if (_workYearName != value)
                {
                    _workYearName = value;
                    OnPropertyChanged(nameof(WorkYearName));
                }
            }
        }

        public string YearName { get; set; }
        public List<DailyWorkHours> DailyWorkHoursList { get; set; }

        public WorkYear(DateTime workYearStartDate, DateTime workYearEndDate, string workYearName)
        {
            WorkYearStartDate = workYearStartDate;
            WorkYearEndDate = workYearEndDate;
            WorkYearName = workYearName;
            DailyWorkHoursList = new List<DailyWorkHours>();
        }

        public bool Overlaps(DateTime startDate, DateTime endDate)
        {
            DateOnly start = DateOnly.FromDateTime(startDate);
            DateOnly end = DateOnly.FromDateTime(endDate);
            return DailyWorkHoursList.Any(dwh => dwh.Date >= start && dwh.Date <= end);
        }

        public bool ContainsDate(DateOnly date)
        {
            return DailyWorkHoursList.Any(dwh => dwh.Date == date);
        }

        public TimeSpan CalculateTotalWorkHours()
        {
            return DailyWorkHoursList.Aggregate(TimeSpan.Zero, (sum, dwh) => sum + dwh.GetTotalWorkHours());
        }

        public double WorkHoursSumByDate(DateTime startDate, DateTime endDate)
        {
            DateOnly start = DateOnly.FromDateTime(startDate);
            DateOnly end = DateOnly.FromDateTime(endDate);

            TimeSpan totalWorkHours = DailyWorkHoursList
                .Where(dwh => dwh.Date >= start && dwh.Date <= end)
                .Aggregate(TimeSpan.Zero, (sum, dwh) => sum + dwh.GetTotalWorkHours());

            return totalWorkHours.TotalHours;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
