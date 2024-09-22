using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using WorkHoursManagementApp.Models;

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

        public void AddWorkYear(DateTime startDate, DateTime endDate, string YearName, decimal hourlyRate)
        {
            WorkYear workYear = new WorkYear(startDate, endDate, YearName, hourlyRate);

            for (DateTime date = startDate; date <= endDate; date = date.AddDays(1))
            {
                // Skip adding work hours for Friday
                if (date.DayOfWeek == DayOfWeek.Friday)
                {
                    workYear.DailyWorkHoursList.Add(new DailyWorkHours
                    {
                        Date = DateOnly.FromDateTime(date),
                        WorkShift = new Models.WorkShiftData
                        {
                            StartTime = DateTime.MinValue, // No work shift
                            EndTime = DateTime.MinValue
                        },
                        MissedTime = new TimeMissedData(DateTime.Today.AddHours(0)) // No missed time for Friday
                    });
                }
                else if (date.DayOfWeek != DayOfWeek.Saturday)
                {
                    var endTime = date.DayOfWeek == DayOfWeek.Tuesday
                        ? DateTime.Today.AddHours(14).AddMinutes(40) // 14:40 on Tuesday
                        : DateTime.Today.AddHours(15.25);            // 15:15 on other weekdays

                    // Set missed time for Sunday, Monday, and Wednesday (2 hours)
                    var missedTime = (date.DayOfWeek == DayOfWeek.Sunday ||
                                      date.DayOfWeek == DayOfWeek.Monday ||
                                      date.DayOfWeek == DayOfWeek.Wednesday)
                        ? new TimeMissedData(DateTime.Today.AddHours(2)) // 2 hours of missed time
                        : new TimeMissedData(DateTime.Today.AddHours(0)); // No missed time on other days

                    workYear.DailyWorkHoursList.Add(new DailyWorkHours
                    {
                        Date = DateOnly.FromDateTime(date),
                        WorkShift = new Models.WorkShiftData
                        {
                            StartTime = DateTime.Today.AddHours(7.5), // 07:30
                            EndTime = endTime
                        },
                        MissedTime = missedTime
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

