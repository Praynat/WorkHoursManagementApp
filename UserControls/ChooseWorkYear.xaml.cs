using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using WorkHoursManagementApp.Pages;

namespace WorkHoursManagementApp.UserControls
{
    public partial class ChooseWorkYear : UserControl
    {
        // Events to signal changes to work years
        public event Action<WorkYear> WorkYearChanged;
        public event Action<WorkYear> WorkYearEdited;
        public event Action<WorkYear> WorkYearDeleted;

        //Event to pass the added year to the home page
        public event Action<WorkYear> AddYearPopupOkClicked;
        //Event to pass the edited year to the home page
        public event Action<WorkYear,string> EditWorkYearPopupOkClicked;

        public ChooseWorkYear()
        {
            InitializeComponent();
            AddWorkYearControl.AddYearPopupOkClicked += OnAddYearPopupOkClicked;
            EditWorkYearControl.EditWorkYearPopupOkClicked+= OnEditYearPopupOkClicked;

            AddWorkYearPopup.Closed += AddWorkYearPopup_Closed;
            EditWorkYearPopup.Closed += EditWorkYearPopup_Closed;

        }

        // Method to load work years into the ListBox
        public void LoadWorkYears(ObservableCollection<WorkYear> workYears)
        {
            WorkYearListBox.ItemsSource = workYears;


        }

        // Event handler for "Add New Work Year" button click
        private void AddWorkYearButton_Click(object sender, RoutedEventArgs e)
        {
            AddWorkYearPopup.IsOpen=true;

            // Hide the ChooseWorkYearPopup
            this.Visibility = Visibility.Collapsed;

        }
        private void OnAddYearPopupOkClicked( DateTime? startDate, DateTime? endDate, string workYearName, decimal hourlyRate)
        {
            if (!string.IsNullOrEmpty(workYearName) && startDate.HasValue && endDate.HasValue && hourlyRate > 0)
            {
                WorkYear newWorkYear = new WorkYear(startDate.Value, endDate.Value, workYearName, hourlyRate);
                AddYearPopupOkClicked?.Invoke(newWorkYear);
            }
                AddWorkYearPopup.IsOpen = false;
            this.Visibility = Visibility.Visible;

        }

        // Event handler for "Change Work Year" button click
        private void ChangeWorkYearButton_Click(object sender, RoutedEventArgs e)
        {
            if (WorkYearListBox.SelectedItem is WorkYear selectedWorkYear)
            {
                WorkYearChanged.Invoke(selectedWorkYear);
            }
            else
            {
                MessageBox.Show("Please select a work year to apply.", "Select Work Year", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        // Event handler for "Edit" button click
        private void EditYearButton_Click(object sender, RoutedEventArgs e)
        {
            if (WorkYearListBox.SelectedItem is WorkYear selectedWorkYear)
            {
                EditWorkYearPopup.IsOpen = true;
                EditWorkYearControl.YearName = selectedWorkYear.WorkYearName;
                EditWorkYearControl.StartDate = selectedWorkYear.WorkYearStartDate;
                EditWorkYearControl.EndDate = selectedWorkYear.WorkYearEndDate;
                EditWorkYearControl.HourlyRateNumericUpDown.Value = selectedWorkYear.HourlyRate;
                this.Visibility = Visibility.Collapsed;
            }
            else
            {
                MessageBox.Show("Please select a work year to edit.", "Select Work Year", MessageBoxButton.OK, MessageBoxImage.Information);
            }



        }

        // Event handler for "Delete" button click
        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (WorkYearListBox.SelectedItem is WorkYear selectedWorkYear)
            {
                var result = MessageBox.Show($"Are you sure you want to delete the work year {selectedWorkYear.WorkYearName}?", "Confirm Delete", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                if (result == MessageBoxResult.Yes)
                {
                    WorkYearDeleted?.Invoke(selectedWorkYear);
                }
            }
            else
            {
                MessageBox.Show("Please select a work year to delete.", "Select Work Year", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
        private void AddWorkYearPopup_Closed(object sender, EventArgs e)
        {
            this.Visibility = Visibility.Visible;
        }
        private void EditWorkYearPopup_Closed(object sender, EventArgs e)
        {
            this.Visibility = Visibility.Visible;
        }
        private void OnEditYearPopupOkClicked(DateTime? startDate, DateTime? endDate, string workYearName, decimal hourlyRate)
        {
            if (!string.IsNullOrEmpty(workYearName) && startDate.HasValue && endDate.HasValue&& WorkYearListBox.SelectedItem is WorkYear selectedWorkYear)
            {

                WorkYear editedWorkYear = new WorkYear(startDate.Value, endDate.Value, workYearName, hourlyRate);
                string selectedYearName = selectedWorkYear.WorkYearName;
                EditWorkYearPopupOkClicked?.Invoke(editedWorkYear,selectedYearName);
                
            }
            else
            {
                MessageBox.Show("Please Enter correct details.", "Enter Details", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            EditWorkYearPopup.IsOpen = false;
            this.Visibility = Visibility.Visible;

        }
    }
}
