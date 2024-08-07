using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using WorkHoursManagementApp.Models;
using WorkHoursManagementApp.Utilities;
using Xceed.Wpf.Toolkit;

namespace WorkHoursManagementApp.Pages
{
    public partial class HomePage : Page
    {
        private User currentUser;
        public DateTime WorkYearStartDate { get; set; } = new DateTime(2023, 9, 1);
        public DateTime WorkYearEndDate { get; set; } = new DateTime(2024, 7, 30);

        public ObservableCollection<DailyWorkHours> DailyWorkHoursItems { get; set; }

        public HomePage()
        {
            InitializeComponent();
            DataContext = this;

            currentUser = new User("Nathan");
            currentUser.AddWorkYear(WorkYearStartDate, WorkYearEndDate);

            DailyWorkHoursItems = new ObservableCollection<DailyWorkHours>();

            myCalendar.SelectedDatesChanged += MyCalendar_SelectedDatesChanged;
            Loaded += HomePage_Loaded;
        }

        private void HomePage_Loaded(object sender, RoutedEventArgs e)
        {
            CalendarUtility.InitializeCalendar(myCalendar, WorkYearStartDate, WorkYearEndDate);
            AttachTimePickerEventHandlers();
        }

        private void AttachTimePickerEventHandlers()
        {
            AttachValueChangedHandler(workShiftControl, "startTimePicker", StartTimePicker_ValueChanged);
            AttachValueChangedHandler(workShiftControl, "endTimePicker", EndTimePicker_ValueChanged);
            AttachValueChangedHandler(extraTimeControl, "extraTimePicker", ExtraTimePicker_ValueChanged);
            AttachValueChangedHandler(timeMissedControl, "timeMissedPicker", TimeMissedPicker_ValueChanged);
        }

        private void AttachValueChangedHandler(Control control, string pickerName, RoutedPropertyChangedEventHandler<object> handler)
        {
            if (control.FindName(pickerName) is TimePicker timePicker)
            {
                timePicker.ValueChanged += handler;
            }
        }

        private void StartTimePicker_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            UpdateWorkShiftTime(sender, e, true);
        }

        private void EndTimePicker_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            UpdateWorkShiftTime(sender, e, false);
        }

        private void UpdateWorkShiftTime(object sender, RoutedPropertyChangedEventArgs<object> e, bool isStartTime)
        {
            if (e.NewValue is DateTime newTime && myCalendar.SelectedDate.HasValue)
            {
                var workDay = CalendarUtility.GetWorkDayForDate(currentUser, myCalendar.SelectedDate.Value);
                if (workDay != null)
                {
                    if (isStartTime)
                        UpdateTime(workDay.WorkShift, newTime, true, sender);
                    else
                        UpdateTime(workDay.WorkShift, newTime, false, sender);
                }
                else
                {
                    System.Windows.MessageBox.Show("No workday found for the selected date.");
                }
            }
        }

        private void UpdateTime(WorkShiftData workShift, DateTime newTime, bool isStartTime, object sender)
        {
            if (isStartTime)
            {
                if (newTime >= workShift.EndTime)
                {
                    System.Windows.MessageBox.Show("Start Time cannot be greater than or equal to End Time. Resetting to End Time.");
                    workShift.StartTime = workShift.EndTime;
                    SetTimePickerValue(sender, workShift.EndTime);
                }
                else
                {
                    workShift.StartTime = newTime;
                }
            }
            else
            {
                if (newTime <= workShift.StartTime)
                {
                    System.Windows.MessageBox.Show("End Time cannot be earlier than or equal to Start Time. Resetting to Start Time.");
                    workShift.EndTime = workShift.StartTime;
                    SetTimePickerValue(sender, workShift.StartTime);
                }
                else
                {
                    workShift.EndTime = newTime;
                }
            }
        }

        private void SetTimePickerValue(object sender, DateTime time)
        {
            if (sender is TimePicker timePicker)
            {
                timePicker.Value = time;
            }
        }

        private void ExtraTimePicker_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            UpdateTimeField(sender, e, time => time.ExtraTime.ExtraTime = (DateTime)e.NewValue, "Extra Time");
        }

        private void TimeMissedPicker_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            UpdateTimeField(sender, e, time => time.MissedTime.TimeMissed = (DateTime)e.NewValue, "Missed Time");
        }

        private void UpdateTimeField(object sender, RoutedPropertyChangedEventArgs<object> e, Action<DailyWorkHours> updateAction, string fieldName)
        {
            if (e.NewValue is DateTime newTime && myCalendar.SelectedDate.HasValue)
            {
                DateTime selectedDate = myCalendar.SelectedDate.Value;
                DailyWorkHours workDay = CalendarUtility.GetWorkDayForDate(currentUser, selectedDate);

                if (workDay != null)
                {
                    updateAction(workDay);
                }
                else
                {
                    System.Windows.MessageBox.Show($"No workday found for {selectedDate:d} to update {fieldName}.");
                }
            }
        }

        private void MyCalendar_SelectedDatesChanged(object sender, SelectionChangedEventArgs e)
        {
            if (myCalendar.SelectedDate.HasValue)
            {
                DateTime selectedDate = myCalendar.SelectedDate.Value;
                UpdateItemsForSelectedDate(selectedDate);
            }
        }

        private void UpdateItemsForSelectedDate(DateTime selectedDate)
        {
            DailyWorkHoursItems.Clear();
            DailyWorkHours workDay = CalendarUtility.GetWorkDayForDate(currentUser, selectedDate);

            if (workDay != null)
            {
                DailyWorkHoursItems.Add(workDay);

                workShiftControl.SetStartTime(workDay.WorkShift.StartTime);
                workShiftControl.SetEndTime(workDay.WorkShift.EndTime);
                extraTimeControl.SetExtraTime(workDay.ExtraTime.ExtraTime);
                timeMissedControl.SetMissedTime(workDay.MissedTime.TimeMissed);
            }
        }
    }
}
