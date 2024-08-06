using System;
using System.Collections.ObjectModel;
using System.Windows.Controls;
using WorkHoursManagementApp.Models;

namespace WorkHoursManagementApp.Pages
{
    public partial class HomePage : Page
    {
        private User currentUser;

        public ObservableCollection<object> Items { get; set; }

        public HomePage()
        {
            InitializeComponent();

            DataContext = this;

            currentUser = new User("Nathan");

            currentUser.AddWorkYear(new DateTime(2023, 9, 1), new DateTime(2024, 7, 30));

            Items = new ObservableCollection<object>();

            myCalendar.SelectedDatesChanged += MyCalendar_SelectedDatesChanged;
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
            Items.Clear();

            WorkYear workYear = currentUser.GetWorkYear(selectedDate);
            if (workYear != null)
            {
                DailyWorkHours workDay = workYear.DailyWorkHoursList.Find(dwh => dwh.Date.Date == selectedDate.Date);
                if (workDay != null)
                {
                    Items.Add(workDay.WorkShift);
                    Items.Add(workDay.ExtraTime);
                    Items.Add(workDay.MissedTime);
                }
            }
        }
    }
}
