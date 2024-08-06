using System;
using WorkHoursManagementApp.Models;

namespace WorkHoursManagementApp
{
    public class DailyWorkHours
    {
        public DateOnly Date { get; set; }
        public WorkShiftData WorkShift { get; set; }
        public ExtraTimeData ExtraTime { get; set; }
        public TimeMissedData MissedTime { get; set; }

        public DailyWorkHours()
        {
            WorkShift = new WorkShiftData();
            ExtraTime = new ExtraTimeData();
            MissedTime = new TimeMissedData();
        }

        public TimeSpan GetTotalWorkHours()
        {
            TimeSpan workShiftDuration = WorkShift.GetWorkShiftDuration();
            TimeSpan extraTimeDuration = ExtraTime.GetExtraTimeDuration();
            TimeSpan missedTimeDuration = MissedTime.GetTimeMissedDuration();

            return workShiftDuration + extraTimeDuration - missedTimeDuration;
        }
    }
}
