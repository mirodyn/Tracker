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
using System.Windows.Shapes;

namespace Tracker.TrackedTasksList
{
    /// <summary>
    /// Interaction logic for TrackedTaskListWindow.xaml
    /// </summary>
    public partial class TrackedTaskListWindow : Window
    {
        public TrackedTaskListWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            TrackedTaskListViewModel model = (TrackedTaskListViewModel)this.DataContext;
            ArchivedList.SelectedIndex = -1;
            UnsyncedList.SelectedIndex = -1;
            model.SelectedDay = model.Today;

        }

        private void ArchivedList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var item = ArchivedList.SelectedItem;
            UnsyncedList.SelectedIndex = -1;
            TrackedTaskListViewModel model = (TrackedTaskListViewModel)this.DataContext;
            model.SelectedDay = (TrackedDayViewModel)item;
        }

        private void UnsyncedList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var item = UnsyncedList.SelectedItem;
            TrackedTaskListViewModel model = (TrackedTaskListViewModel)this.DataContext;
            model.SelectedDay = (TrackedDayViewModel)item;
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            TrackedTaskListViewModel model = (TrackedTaskListViewModel)this.DataContext;
            //model.
            
        }


    }
}
