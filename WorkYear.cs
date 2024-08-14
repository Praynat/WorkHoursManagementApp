using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using WorkHoursManagementApp.UserControls;

namespace WorkHoursManagementApp
{
    public class WorkYear : INotifyPropertyChanged
    {
        private DateTime _workYearStartDate;
        private DateTime _workYearEndDate;
        private string _workYearName;
        private decimal _hourlyRate { get; set; }


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
        public decimal HourlyRate
        {
            get => _hourlyRate;
            set
            {
                if (_hourlyRate != value)
                {
                    _hourlyRate = value;
                    OnPropertyChanged(nameof(HourlyRate));
                }
            }
        }



        public string YearName { get; set; }
        public List<DailyWorkHours> DailyWorkHoursList { get; set; }

        public WorkYear(DateTime workYearStartDate, DateTime workYearEndDate, string workYearName,decimal hourlyRate)
        {
            WorkYearStartDate = workYearStartDate;
            WorkYearEndDate = workYearEndDate;
            WorkYearName = workYearName;
            DailyWorkHoursList = new List<DailyWorkHours>();
            HourlyRate = hourlyRate;
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
