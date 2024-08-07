using System;
using System.Windows;
using System.Windows.Controls;

namespace WorkHoursManagementApp.UserControls
{
    
    public partial class TimeMissedControl : UserControl
    {
        public DateTime TimeMissed
        {
            get { return (DateTime)GetValue(TimeMissedProperty); }
            set { SetValue(TimeMissedProperty, value); }
        }

        public static readonly DependencyProperty TimeMissedProperty =
            DependencyProperty.Register("TimeMissed", typeof(DateTime), typeof(TimeMissedControl), new PropertyMetadata(DateTime.Now));

        public TimeMissedControl()
        {
            InitializeComponent();
            this.DataContext = this;

            
            TimeMissed = DateTime.Today.AddHours(0);
        }

        public void SetMissedTime(DateTime newTime)
        {
            TimeMissed = newTime;
        }
    }
}
