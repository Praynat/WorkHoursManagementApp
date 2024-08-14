using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;

namespace WorkHoursManagementApp
{
    public class User
    {
        public string Username { get; set; }
        public ObservableCollection<WorkYear> WorkYearsList { get; set; }

        public User(string username)
        {
            Username = username;
            WorkYearsList = new ObservableCollection<WorkYear>();
        }

        public void AddWorkYear(DateTime startDate, DateTime endDate,string YearName)
        {         
                     WorkYear workYear = new WorkYear(startDate,endDate,YearName);

                for (DateTime date = startDate; date <= endDate; date = date.AddDays(1))
                {
                    if (date.DayOfWeek != DayOfWeek.Saturday)
                    {
                        workYear.DailyWorkHoursList.Add(new DailyWorkHours
                        {
                            Date = DateOnly.FromDateTime(date),
                            WorkShift = new Models.WorkShiftData { StartTime = DateTime.Today.AddHours(9), EndTime = DateTime.Today.AddHours(17) }
                        });
                    }
                }

                WorkYearsList.Add(workYear);
        }

        public WorkYear GetWorkYearByDate(DateTime date)
        {
            var dateOnly = DateOnly.FromDateTime(date); // Convert DateTime to DateOnly for comparison
            return WorkYearsList.FirstOrDefault(wy => wy.ContainsDate(dateOnly));
        }

        public WorkYear GetWorkYearByName(string yearName)
        {
            return WorkYearsList.FirstOrDefault(wy => wy.WorkYearName == yearName);
        }

        public TimeSpan GetTotalWorkHoursFromDates(DateTime startDate, DateTime endDate)
        {
            TimeSpan totalHours = TimeSpan.Zero;

            DateOnly start = DateOnly.FromDateTime(startDate); // Convert to DateOnly
            DateOnly end = DateOnly.FromDateTime(endDate);     // Convert to DateOnly

            foreach (WorkYear workYear in WorkYearsList)
            {
                IEnumerable<DailyWorkHours> relevantDays = workYear.DailyWorkHoursList
                    .Where(dwh => dwh.Date >= start && dwh.Date <= end); // Use DateOnly for comparison

                foreach (DailyWorkHours day in relevantDays)
                {
                    totalHours += day.GetTotalWorkHours();
                }
            }

            return totalHours;
        }
    }


}

