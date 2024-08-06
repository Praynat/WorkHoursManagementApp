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
                            Date = date,
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
            return WorkYearsList.FirstOrDefault(wy => wy.ContainsDate(date));
        }

        public TimeSpan GetTotalWorkHoursFromDates(DateTime startDate, DateTime endDate)
        {
            TimeSpan totalHours = TimeSpan.Zero;

            foreach (WorkYear workYear in WorkYearsList)
            {
                IEnumerable<DailyWorkHours> relevantDays = workYear.DailyWorkHoursList
                    .Where(dwh => dwh.Date >= startDate && dwh.Date <= endDate);

                foreach (DailyWorkHours day in relevantDays)
                {
                    totalHours += day.GetTotalWorkHours();
                }
            }

            return totalHours;
        }
    }


}

