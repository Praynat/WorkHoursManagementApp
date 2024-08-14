using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;

namespace WorkHoursManagementApp.Utilities
{
    public static class CalendarUtility
    {
        public static List<DateTime> GetBlackoutDates(Calendar calendar, DateTime workYearStartDate, DateTime workYearEndDate, DayOfWeek dayOfWeek=DayOfWeek.Saturday)
        {
            calendar.BlackoutDates.Clear();

            List<DateTime> allBlackoutDates = new List<DateTime>();

            for (DateTime date = workYearStartDate; date <= workYearEndDate; date = date.AddDays(1))
            {
                if (date.DayOfWeek == dayOfWeek)
                {
                    allBlackoutDates.Add(date);
                }
            }

            return allBlackoutDates;
        }

        public static void HighlightBlackoutDates(Calendar calendar, List<DateTime> blackoutDates)
        {
            calendar.DisplayDateChanged += (s, e) =>
            {
                UpdateCalendarStyle(calendar, blackoutDates);
            };
        }

        public static void UpdateCalendarStyle(Calendar calendar, List<DateTime> blackoutDates)
        {
            foreach (var dayButton in FindVisualChildren<CalendarDayButton>(calendar))
            {
                DateTime date;
                if (DateTime.TryParse(dayButton.DataContext.ToString(), out date))
                {
                    dayButton.Tag = blackoutDates.Contains(date);
                }
            }
        }

        private static IEnumerable<T> FindVisualChildren<T>(DependencyObject depObj) where T : DependencyObject
        {
            if (depObj != null)
            {
                for (int i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++)
                {
                    DependencyObject child = VisualTreeHelper.GetChild(depObj, i);
                    if (child != null && child is T)
                    {
                        yield return (T)child;
                    }

                    foreach (T childOfChild in FindVisualChildren<T>(child))
                    {
                        yield return childOfChild;
                    }
                }
            }
        }

        public static DailyWorkHours GetWorkDayForDate(User currentUser, DateTime selectedDate)
        {
            WorkYear workYear = currentUser.GetWorkYearByDate(selectedDate);
            if (workYear != null)
            {
                DateOnly selectedDateOnly = DateOnly.FromDateTime(selectedDate);
                return workYear.DailyWorkHoursList.Find(dwh => dwh.Date == selectedDateOnly);
            }
            return null;
        }

        public static List<DailyWorkHours> GetWorkDaysForPeriod(User currentUser, DateTime startDate, DateTime endDate)
        {
            List<DailyWorkHours> workDays = new List<DailyWorkHours>();

            DateOnly start = DateOnly.FromDateTime(startDate);
            DateOnly end = DateOnly.FromDateTime(endDate);

            foreach (var workYear in currentUser.WorkYearsList)
            {
                var relevantDays = workYear.DailyWorkHoursList
                    .Where(dwh => dwh.Date >= start && dwh.Date <= end);

                workDays.AddRange(relevantDays);
            }

            return workDays;
        }
    }
}
