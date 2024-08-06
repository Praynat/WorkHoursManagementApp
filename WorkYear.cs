using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkHoursManagementApp
{
    public class WorkYear
    {
        public int Year { get; set; }
        public List<DailyWorkHours> DailyWorkHoursList { get; set; }

        public WorkYear(int year)
        {
            Year = year;
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
    }


}
