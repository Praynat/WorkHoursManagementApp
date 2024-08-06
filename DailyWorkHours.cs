using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkHoursManagementApp.Models;

namespace WorkHoursManagementApp
{
    public class DailyWorkHours
    {
        public DateTime Date { get; set; }
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
            return WorkShift.GetWorkShiftDuration() + ExtraTime.ExtraTime - MissedTime.TimeMissed;
        }
    }
}
