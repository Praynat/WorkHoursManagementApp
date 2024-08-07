using System;
using System.Collections.ObjectModel;
using System.Data;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Animation;
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
        public string WorkYearName { get; set; } = "Yeshivat Noam 2023-2024";

        public ObservableCollection<DailyWorkHours> DailyWorkHoursItems { get; set; }

        public HomePage()
        {
            InitializeComponent();
            DataContext = this;

            currentUser = new User("Nathan");
            currentUser.AddWorkYear(WorkYearStartDate, WorkYearEndDate, "Yeshivat Noam 2023-2024");

            DailyWorkHoursItems = new ObservableCollection<DailyWorkHours>();

            myCalendar.SelectedDatesChanged += MyCalendar_SelectedDatesChanged;
            Loaded += HomePage_Loaded;
        }

        private void HomePage_Loaded(object sender, RoutedEventArgs e)
        {
            //Utility Functions that defines blackout dates and a function that returns a DailyWorkHours by date
            CalendarUtility.InitializeCalendar(myCalendar, WorkYearStartDate, WorkYearEndDate);

            //Attaches Functions to time picker events
            AttachTimePickerEventHandlers();
        }

        //Attaches Functions to time picker events
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

        //Updates the data when time is changed by user
        private void StartTimePicker_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            UpdateWorkShiftTime(sender, e, true);
        }

        private void EndTimePicker_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            UpdateWorkShiftTime(sender, e, false);
        }

        //Updates the data when time is changed by user
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

        //Complementary function that Updates the data when time is changed by user
        private void UpdateTime(WorkShiftData workShift, DateTime newTime, bool isStartTime, object sender)
        {
            if (isStartTime)
            {
                if (newTime > workShift.EndTime)
                {
                    System.Windows.MessageBox.Show("Start Time cannot be greater than End Time. Resetting to End Time.");
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
                if (newTime < workShift.StartTime)
                {
                    System.Windows.MessageBox.Show("End Time cannot be smaller than Start Time. Resetting to Start Time.");
                    workShift.EndTime = workShift.StartTime;
                    SetTimePickerValue(sender, workShift.StartTime);
                }
                else
                {
                    workShift.EndTime = newTime;
                }
            }
        }
        //Complementary method for previous Update time method
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

        //Update the time showing up in time pickers when selecting a date
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

        private void Grid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (MenuPanel.Visibility == Visibility.Visible)
            {
                
                if (!MenuPanel.IsMouseOver)
                {
                    CloseMenu();
                }
            }
        }

        private void ToggleMenu(object sender, RoutedEventArgs e)
        {
            if (MenuPanel.Visibility == Visibility.Collapsed)
            {
                OpenMenu();
            }
            else
            {
                CloseMenu();
            }
        }

        private void OpenMenu()
        {
            MenuPanel.Visibility = Visibility.Visible;
            Storyboard openMenuAnimation = (Storyboard)FindResource("OpenMenuAnimation");
            openMenuAnimation.Begin();
        }

        private void CloseMenu()
        {
            Storyboard closeMenuAnimation = (Storyboard)FindResource("CloseMenuAnimation");
            closeMenuAnimation.Completed += (s, _) => MenuPanel.Visibility = Visibility.Collapsed;
            closeMenuAnimation.Begin();
        }


        private void Button_Click(object sender, RoutedEventArgs e)
        {
            DateTime firstDate= new DateTime(2023, 9, 1);
            DateTime lastDate = new DateTime(2023, 10, 1);
            WorkYear selectedWorkYear = currentUser.GetWorkYearByName("Yeshivat Noam 2023-2024");
            double totalHours=selectedWorkYear.WorkHoursSumByDate(firstDate, lastDate);
            System.Windows.MessageBox.Show($"Total work hours for {selectedWorkYear.YearName} from {firstDate:d} to {lastDate:d}: {totalHours} hours");
        }
    }
}
