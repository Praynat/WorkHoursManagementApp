﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace WorkHoursManagementApp.Utilities
{
    public static class CalendarUtility
    {
        public static void InitializeCalendar(Calendar calendar, DateTime workYearStartDate, DateTime workYearEndDate)
        {
            calendar.BlackoutDates.Clear();
            calendar.BlackoutDates.Add(new CalendarDateRange(DateTime.MinValue, workYearStartDate.AddDays(-1)));
            calendar.BlackoutDates.Add(new CalendarDateRange(workYearEndDate.AddDays(1), DateTime.MaxValue));

            for (DateTime date = workYearStartDate; date <= workYearEndDate; date = date.AddDays(1))
            {
                if (date.DayOfWeek == DayOfWeek.Saturday)
                {
                    calendar.BlackoutDates.Add(new CalendarDateRange(date));
                }
            }
        }

        public static DailyWorkHours GetWorkDayForDate(User currentUser, DateTime selectedDate)
        {
            WorkYear workYear = currentUser.GetWorkYear(selectedDate);
            if (workYear != null)
            {
                DateOnly selectedDateOnly = DateOnly.FromDateTime(selectedDate);
                return workYear.DailyWorkHoursList.Find(dwh => dwh.Date == selectedDateOnly);
            }
            return null;
        }
    }
}