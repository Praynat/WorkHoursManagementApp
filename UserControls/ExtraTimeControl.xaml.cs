using System;
using System.Windows;
using System.Windows.Controls;

namespace WorkHoursManagementApp.UserControls
{
    
    public partial class ExtraTimeControl : UserControl
    {
        public DateTime ExtraTime
        {
            get { return (DateTime)GetValue(ExtraTimeProperty); }
            set { SetValue(ExtraTimeProperty, value); }
        }

        public static readonly DependencyProperty ExtraTimeProperty =
            DependencyProperty.Register("ExtraTime", typeof(DateTime), typeof(ExtraTimeControl), new PropertyMetadata(DateTime.Now));

        public ExtraTimeControl()
        {
            InitializeComponent();
            this.DataContext = this;

            
            ExtraTime = DateTime.Today.AddHours(0);
        }

        public void SetExtraTime(DateTime newTime)
        {
            ExtraTime = newTime;
        }
    }
}
