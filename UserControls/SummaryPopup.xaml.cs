using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace WorkHoursManagementApp.UserControls
{
    public partial class SummaryPopup : UserControl, INotifyPropertyChanged
    {
        private decimal _hourlyRate;
        private double _totalHours;
        private string _startDate;
        private string _endDate;
        private double _salary;

        public event PropertyChangedEventHandler PropertyChanged;

        public Action<decimal> OnHourlyRateTextboxChange;

        // Bindable properties
        public decimal HourlyRate
        {
            get => _hourlyRate;
            set
            {
                if (_hourlyRate != value)
                {
                    _hourlyRate = value;
                    OnPropertyChanged(nameof(HourlyRate));
                    UpdateSalary();
                }
            }
        }

        public double TotalHours
        {
            get => _totalHours;
            set
            {
                if (_totalHours != value)
                {
                    _totalHours = value;
                    OnPropertyChanged(nameof(TotalHours));
                    UpdateSalary();
                }
            }
        }

        public string StartDate
        {
            get => _startDate;
            set
            {
                if (_startDate != value)
                {
                    _startDate = value;
                    OnPropertyChanged(nameof(StartDate));
                    UpdateDisplayText();
                }
            }
        }

        public string EndDate
        {
            get => _endDate;
            set
            {
                if (_endDate != value)
                {
                    _endDate = value;
                    OnPropertyChanged(nameof(EndDate));
                    UpdateDisplayText();
                }
            }
        }

        public double Salary
        {
            get => _salary;
            private set
            {
                if (_salary != value)
                {
                    _salary = value;
                    OnPropertyChanged(nameof(Salary));
                }
            }
        }

        public SummaryPopup()
        {
            InitializeComponent();
            DataContext = this;
        }

        private void UpdateSalary()
        {
            Salary = (double)((decimal)TotalHours * HourlyRate);
        }
        public void UpdatehourlyRate(decimal rate)
        {
            HourlyRate = rate;
        }
        private void UpdateDisplayText()
        {
            HoursSummaryText.Text = $"The amount of hours you have worked from \n {StartDate} to {EndDate} is:";
            TotalHoursText.Text = $"{TotalHours:F2} hours";
            TotalSalaryText.Text = $"The amount of money you received is: ₪{Salary:F2}";
        }

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void HourlyRateTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

            UpdateSalary();
            UpdateDisplayText() ;
            if (decimal.TryParse(HourlyRateTextBox.Text, out decimal rate))
            {
                OnHourlyRateTextboxChange?.Invoke(rate);
            }
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            // Close the parent popup if it exists
            var popup = this.Parent as Popup;
            if (popup != null)
            {
                popup.IsOpen = false;
            }
        }
    }
}
