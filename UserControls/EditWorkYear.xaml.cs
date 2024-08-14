using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WorkHoursManagementApp.UserControls
{
    public partial class EditWorkYear : UserControl, INotifyPropertyChanged
    {
        private DateTime? _startDate;
        private DateTime? _endDate;
        private string _yearName;
        private decimal _editHourlyRate;

        public event PropertyChangedEventHandler PropertyChanged;

        public event Action<DateTime?, DateTime?, string,decimal> EditWorkYearPopupOkClicked;

        public EditWorkYear()
        {
            InitializeComponent();
            DataContext = this;
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
        public decimal EditHourlyRate
        {
            get => _editHourlyRate;
            set
            {
                if (_editHourlyRate != value)
                {
                    _editHourlyRate = value;
                    OnPropertyChanged(nameof(EditHourlyRate));
                }
            }
        }

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        private void OkButton_Click(object sender, RoutedEventArgs e)
        {

            EditWorkYearPopupOkClicked?.Invoke(StartDate, EndDate, YearName,EditHourlyRate);
            

            YearName = string.Empty;
            StartDate = null;
            EndDate = null;
            
        }
    }
}
