using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

namespace WorkHoursManagementApp.UserControls
{
    public partial class NewYearControl : UserControl, INotifyPropertyChanged
    {
        private DateTime? _startDate;
        private DateTime? _endDate;
        private string _yearName;
        private decimal _newHourlyRate;

        public event PropertyChangedEventHandler PropertyChanged;

        public event Action< DateTime?, DateTime?, string,decimal> AddYearPopupOkClicked;

        public NewYearControl()
        {
            InitializeComponent();
            DataContext = this;
        }

        public string YearName
        {
            get => _yearName;
            set
            {
                if (_yearName != value)
                {
                    _yearName = value;
                    OnPropertyChanged(nameof(YearName));
                }
            }
        }

        public DateTime? StartDate
        {
            get => _startDate;
            set
            {
                if (_startDate != value)
                {
                    _startDate = value;
                    OnPropertyChanged(nameof(StartDate));
                }
            }
        }

        public DateTime? EndDate
        {
            get => _endDate;
            set
            {
                if (_endDate != value)
                {
                    _endDate = value;
                    OnPropertyChanged(nameof(EndDate));
                }
            }
        }
        public decimal NewHourlyRate
        {
            get => _newHourlyRate;
            set
            {
                if (_newHourlyRate != value)
                {
                    _newHourlyRate = value;
                    OnPropertyChanged(nameof(NewHourlyRate));
                }
            }
        }

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

       
        private void OkButton_Click(object sender, RoutedEventArgs e)
        {

            AddYearPopupOkClicked?.Invoke(StartDate, EndDate, YearName,NewHourlyRate);

     
            YearName = string.Empty;
            StartDate = null;
            EndDate = null;
            NewHourlyRate = 0;     
        }
    }
}
