using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkHoursManagementApp
{
    public class User
    {
        public string Username { get; set; }
        public List<WorkYear> WorkYearsList { get; set; }

        public User(string username)
        {
            Username = username;
            WorkYearsList = new List<WorkYear>();
        }

        public void AddWorkYear(DateTime startDate, DateTime endDate)
        {
            if (!WorkYearsList.Any(wy => wy.Overlaps(startDate, endDate)))
            {
                WorkYear workYear = new WorkYear(startDate.Year);

                for (DateTime date = startDate; date <= endDate; date = date.AddDays(1))
                {
                    if (date.DayOfWeek != DayOfWeek.Saturday)
                    {
                        workYear.DailyWorkHoursList.Add(new DailyWorkHours
                        {
                            Date = DateOnly.FromDateTime(date),
                            WorkShift = new Models.WorkShiftData { StartTime = new TimeSpan(9, 0, 0), EndTime = new TimeSpan(17, 0, 0) }
                        });
                    }
                }

                WorkYearsList.Add(workYear);
            }
            else
            {
                Console.WriteLine($"A work period overlapping with these dates already exists for the user {Username}.");
            }
        }

        public WorkYear GetWorkYear(DateTime date)
        {
            var dateOnly = DateOnly.FromDateTime(date); // Convert DateTime to DateOnly for comparison
            return WorkYearsList.FirstOrDefault(wy => wy.ContainsDate(dateOnly));
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

