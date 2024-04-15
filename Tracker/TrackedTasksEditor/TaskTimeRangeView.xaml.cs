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

namespace Tracker.TrackedTasksEditor
{
    /// <summary>
    /// Interaction logic for TaskTimeRangeView.xaml
    /// </summary>
    public partial class TaskTimeRangeView : UserControl
    {

        public static readonly DependencyProperty ScaleProperty =
            DependencyProperty.Register("Scale", typeof(double), typeof(TaskTimeRangeView));

        public double Scale
        {
            get
            {
                return (double) GetValue(ScaleProperty);
            }
            set
            {
            SetValue(ScaleProperty, value);
            }
        }

        public static readonly DependencyProperty YPositionProperty =
          DependencyProperty.Register("YPosition", typeof(double), typeof(TaskTimeRangeView));

        public double YPosition
        {
            get
            {
                return (double)GetValue(YPositionProperty);
            }
            set
            {
                SetValue(YPositionProperty, value);
            }
        }


        public TaskTimeRangeView()
        {
            InitializeComponent();
        }
    }
}
