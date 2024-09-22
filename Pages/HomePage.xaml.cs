using System;
using System.Collections.ObjectModel;
using System.Data;
using System.IO;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Xml;
using WorkHoursManagementApp.Models;
using WorkHoursManagementApp.UserControls;
using WorkHoursManagementApp.Utilities;
using Xceed.Wpf.Toolkit;


namespace WorkHoursManagementApp.Pages
{
    public partial class HomePage : Page
    {
        private User currentUser;
        private string userDataFilePath = "userData.json";

        public ObservableCollection<DailyWorkHours> DailyWorkHoursItems { get; set; }
        private List<DateTime> allBlackoutDates;
        private WorkYear CurrentWorkYear { get; set; }

        public HomePage()
        {
            InitializeComponent();
            DataContext = this;

            Application.Current.MainWindow.Closing += OnWindowClosing;

            // Load user data from JSON file
            currentUser = LoadUserData(userDataFilePath) ?? new User("Nathan");

            //creating user and workyear if empty for demo
            if (currentUser.WorkYearsList == null || !currentUser.WorkYearsList.Any())
            {   
            currentUser = new User("Nathan");
            currentUser.AddWorkYear(new DateTime(2024, 9, 1), new DateTime(2025, 7, 31), "Yeshivat Noam 2024-2025", 32.3m);
            currentUser.AddWorkYear(new DateTime(2023, 9, 1), new DateTime(2024, 7, 31), "Yeshivat Noam 2023-2024",42);
            currentUser.AddWorkYear(new DateTime(2022, 10, 1), new DateTime(2023, 8, 30), "Yeshivat Noam 2022-2023",32);
            currentUser.AddWorkYear(new DateTime(2021, 11, 1), new DateTime(2022, 9, 30), "Yeshivat Noam 2021-2022",31.9m);
            }

            //Methods passed on from the ChooseWorkYear Control for choosing and adding WorkYears
            ChooseWorkYearControl.LoadWorkYears(currentUser.WorkYearsList);
            ChooseWorkYearControl.WorkYearChanged += ChooseWorkYear_WorkYearChanged;
            ChooseWorkYearControl.WorkYearDeleted+= ChooseWorkYear_WorkYearDeleted;

            ChooseWorkYearControl.AddYearPopupOkClicked += OnNewWorkYearAdded;
            ChooseWorkYearControl.EditWorkYearPopupOkClicked += OnNewWorkYearEdited;

            //Takes in hand the basic function of updating the workhours
            DailyWorkHoursItems = new ObservableCollection<DailyWorkHours>();
            myCalendar.SelectedDatesChanged += MyCalendar_SelectedDatesChanged;
            AttachTimePickerEventHandlers();

            //Takes in hand the summary popup
            var dateRangeSelector = new DateRangeSelector();
            dateRangeSelector.DatesSelected += DateRangeSelector_DatesSelected;
            DateRangePopup.Child = dateRangeSelector;
            SummaryPopupControl.OnHourlyRateTextboxChange += UpdateWorkYearHourlyRate;
        }

        private void OnWindowClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            SaveUserData(currentUser, userDataFilePath);
        }
        private void SaveUserData(User user, string filePath)
        {
            var options = new JsonSerializerOptions
            {
                WriteIndented = true 
            };

            var jsonData = JsonSerializer.Serialize(user, options);
            File.WriteAllText(filePath, jsonData);
        }

        private User LoadUserData(string filePath)
        {
            if (!File.Exists(filePath))
                return null;

            var jsonData = File.ReadAllText(filePath);
            var user = JsonSerializer.Deserialize<User>(jsonData);
            return user;
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


        private void ShowSummaryButton_Click(object sender, RoutedEventArgs e)
        {
            var dateRangeSelector = DateRangePopup.Child as DateRangeSelector;
            if (dateRangeSelector != null)
            {
                dateRangeSelector.SetDefaultDateRange(myCalendar.DisplayDate);
            }
            DateRangePopup.IsOpen = true;
        }
        private void UpdateWorkYearHourlyRate(decimal newRate)
        {
            if (CurrentWorkYear != null)
            {
                CurrentWorkYear.HourlyRate = newRate;
            }
        }
        private void DateRangeSelector_DatesSelected(DateTime? startDate, DateTime? endDate)
        {
                
            if (startDate.HasValue && endDate.HasValue)
            {
                if (CurrentWorkYear != null)
                {
                    double totalHours = CurrentWorkYear.WorkHoursSumByDate(startDate.Value, endDate.Value);

                    SummaryPopupControl.TotalHours = totalHours;
                    SummaryPopupControl.StartDate = startDate.Value.ToString("d");
                    SummaryPopupControl.EndDate = endDate.Value.ToString("d");

                    SummaryPopup.IsOpen = true;
                }
                else
                {
                    System.Windows.MessageBox.Show("Work year not found.");
                }
            }
            else
            {
                System.Windows.MessageBox.Show("Invalid date selection.");
            }
        }

        private void ChangeWorkYearButton_Click(object sender, RoutedEventArgs e)
        {
            ChooseWorkYearPopup.IsOpen = true;
        }
        private void ChooseWorkYear_WorkYearChanged(WorkYear selectedWorkYear)
        {
            ApplyCurrentWorkYear(selectedWorkYear);
            CurrentWorkYear = selectedWorkYear;
            SummaryPopupControl.UpdatehourlyRate(selectedWorkYear.HourlyRate);
            //System.Windows.MessageBox.Show($"The Hourly rate was updated to {selectedWorkYear.HourlyRate} for {CurrentWorkYear.WorkYearName}");
        }
        public void ApplyCurrentWorkYear(WorkYear currentWorkYear)
        {
                  
            DateTime workYearStartDate = currentWorkYear.WorkYearStartDate;
            DateTime workYearEndDate = currentWorkYear.WorkYearEndDate;

            //Defines Calendar start and end dates
            myCalendar.DisplayDateStart = workYearStartDate;
            myCalendar.DisplayDateEnd = workYearEndDate;
            myCalendar.SelectedDate = (DateTime.Today >= workYearStartDate && DateTime.Today <= workYearEndDate) ? DateTime.Today : workYearStartDate;

            //Defines Dates to Blockout
            allBlackoutDates = CalendarUtility.GetBlackoutDates(myCalendar, workYearStartDate, workYearEndDate);
            CalendarUtility.HighlightBlackoutDates(myCalendar, allBlackoutDates);
            CalendarUtility.UpdateCalendarStyle(myCalendar, allBlackoutDates);

            ChooseWorkYearPopup.IsOpen=false;
        }

        private void ChooseWorkYear_WorkYearDeleted(WorkYear selectedWorkYear)
        {
            currentUser.WorkYearsList.Remove(selectedWorkYear);
            string message = $"The work year '{selectedWorkYear.WorkYearName}' has been deleted.";

            // Update the calendar based on the remaining work years
            if (currentUser.WorkYearsList.Count > 0)
            {
                // Apply the first available work year
                WorkYear currentWorkYear = currentUser.WorkYearsList[0];
                ApplyCurrentWorkYear(currentWorkYear);

                // Add information about the new year being displayed to the message
                message += $"\nThe calendar will now show the work year '{currentWorkYear.WorkYearName}'.";
            }
            else
            {
                // No work years left, reset the calendar
                myCalendar.DisplayDateStart = null;
                myCalendar.DisplayDateEnd = null;
                myCalendar.SelectedDate = null;

                // Add information about the calendar reset to the message
                message += "\nNo work years available. The calendar has been reset.";
            }

            // Show the combined message
            System.Windows.MessageBox.Show(message, "Year Deleted", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        private void OnNewWorkYearAdded(WorkYear newWorkYear)
        {
            currentUser.AddWorkYear(newWorkYear.WorkYearStartDate, newWorkYear.WorkYearEndDate,newWorkYear.WorkYearName,  newWorkYear.HourlyRate);
        }
        public void SetEditWorkYearData(WorkYear CurrentWorkYear)
        {
            ChooseWorkYearControl.EditWorkYearControl.YearName = CurrentWorkYear.WorkYearName;
            ChooseWorkYearControl.EditWorkYearControl.StartDate = CurrentWorkYear.WorkYearStartDate;
            ChooseWorkYearControl.EditWorkYearControl.EndDate = CurrentWorkYear.WorkYearEndDate;
        }
        private void OnNewWorkYearEdited(WorkYear editedWorkYear,string nameOfYearToEdit)
        {
           WorkYear workYearToReplace= currentUser.GetWorkYearByName(nameOfYearToEdit);
            if (workYearToReplace != null) 
            {
                workYearToReplace.WorkYearStartDate = editedWorkYear.WorkYearStartDate;
                workYearToReplace.WorkYearEndDate = editedWorkYear.WorkYearEndDate;
                workYearToReplace.WorkYearName = editedWorkYear.WorkYearName; 
                workYearToReplace.HourlyRate = editedWorkYear.HourlyRate;
                ApplyCurrentWorkYear(workYearToReplace);
            }
        }

        private void WorkHourFeeButton_Click(object sender, RoutedEventArgs e)
        {
            HourlyRatePopup.IsOpen = true;
        }
        private void HourlyRateOkButton_Click(object sender, RoutedEventArgs e)
        {
            
            decimal newHourlyRate= HourlyRateNumericUpDown.Value ?? 0;
            CurrentWorkYear.HourlyRate=newHourlyRate;
            SummaryPopupControl.UpdatehourlyRate(newHourlyRate);
            System.Windows.MessageBox.Show($"The Hourly rate was updated to{newHourlyRate}");
            
        }
    }

    }

