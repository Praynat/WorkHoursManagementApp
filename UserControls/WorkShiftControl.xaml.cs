using System;
using System.Collections.Generic;
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
   
    public partial class WorkShiftControl : UserControl
    {
        public DateTime StartTime
        {
            get { return (DateTime)GetValue(StartTimeProperty); }
            set { SetValue(StartTimeProperty, value); }
        }

        public static readonly DependencyProperty StartTimeProperty =
            DependencyProperty.Register("StartTime", typeof(DateTime), typeof(WorkShiftControl), new PropertyMetadata(DateTime.Now));

        public DateTime EndTime
        {
            get { return (DateTime)GetValue(EndTimeProperty); }
            set { SetValue(EndTimeProperty, value); }
        }

        public static readonly DependencyProperty EndTimeProperty =
            DependencyProperty.Register("EndTime", typeof(DateTime), typeof(WorkShiftControl), new PropertyMetadata(DateTime.Now));
        

        public WorkShiftControl()
        {
            InitializeComponent();
            this.DataContext = this;

            StartTime = new DateTime(2024, 1, 1, 9, 0, 0); // 9:00 AM
            EndTime = new DateTime(2024, 1, 1, 17, 0, 0); // 5:00 PM


        }
        public void SetStartTime(DateTime newTime)
        {
            StartTime = newTime;
        }

        public void SetEndTime(DateTime newTime)
        {
            EndTime = newTime;
        }
    }
}