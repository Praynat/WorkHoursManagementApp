using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace WorkHoursManagementApp.UserControls
{
    public partial class DateRangeSelector : UserControl, INotifyPropertyChanged
    {
        private DateTime? _startDate;
        private DateTime? _endDate;

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

        public event Action<DateTime?, DateTime?> DatesSelected;
        public event PropertyChangedEventHandler PropertyChanged;

        public DateRangeSelector()
        {
            InitializeComponent();
            DataContext = this;
        }

        public void SetDefaultDateRange(DateTime displayDate)
        {
            StartDate = new DateTime(displayDate.Year, displayDate.Month, 1);
            EndDate = new DateTime(displayDate.Year, displayDate.Month, DateTime.DaysInMonth(displayDate.Year, displayDate.Month));
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            DatesSelected?.Invoke(StartDate, EndDate);
            if (this.Parent is Popup parentPopup)
            {
                parentPopup.IsOpen = false;
            }
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
