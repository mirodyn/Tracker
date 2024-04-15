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
using System.Windows.Threading;
using Tracker.TrackedTasksEditor;
using Tracker.TrackedTasksList;

namespace Tracker
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            TrackerViewModel model = new TrackerViewModel();
            this.DataContext = model;
            if (model.Configuration.ConfigReady)
            {
                MainGrid.Visibility = Visibility.Visible;
                SettingsGrid.Visibility = Visibility.Collapsed;
            }
            else
            {
                MainGrid.Visibility = Visibility.Collapsed;
                SettingsGrid.Visibility = Visibility.Visible;
            }
            PassBox.Password = model.Configuration.QiConfig.Password;
        }

        DispatcherTimer _timer;
        TimeSpan _time;

        private void ItemsControl_Drop(object sender, DragEventArgs e)
        {
            if (_timer != null) _timer.Stop(); 
            _time = TimeSpan.FromSeconds(0);

            _timer = new DispatcherTimer(new TimeSpan(0, 0, 1), DispatcherPriority.Normal, delegate
            {
                //todo tady by to chtelo upravit tak, aby se zobrazoval cas od startu mereni.
                //Protoze pokud se pocitac uspi, cas ne nepricita a zobrazeny cas neodpovida tomu
                //co se ulozi do seznamu TimeRanges...
                TimerTb.Text = _time.ToString("c");
                _time = _time.Add(TimeSpan.FromSeconds(1));
            }, Application.Current.Dispatcher);

            _timer.Start();

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (_timer != null) _timer.Stop();
            TimerTb.Text = "00:00:00";
           TrackerViewModel model = (TrackerViewModel)this.DataContext;
            model.FinishCurrentTask();

        }

        private void EditorBtn_Click(object sender, RoutedEventArgs e)
        {
            TrackedTasksEditorWindow editor = new TrackedTasksEditorWindow();
            editor.DataContext = this.DataContext;
            editor.Show();
        }

        private void SettingBtn_Click(object sender, RoutedEventArgs e)
        {
            MainGrid.Visibility = Visibility.Collapsed;
            SettingsGrid.Visibility = Visibility.Visible;
        }

        private void SaveSettingsBtn_Click(object sender, RoutedEventArgs e)
        {
            MainGrid.Visibility = Visibility.Visible;
            SettingsGrid.Visibility = Visibility.Collapsed;
            TrackerViewModel model = (TrackerViewModel)this.DataContext;
            model.Configuration.SaveToJson();
            model.ReloadQiTasks();
        }

        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (this.DataContext != null)
            { ((TrackerViewModel)this.DataContext).Configuration.QiConfig.Password = ((PasswordBox)sender).Password; }
            //((TrackerViewModel)this.DataContext)
        }

        private void TrackedListBtn_Click(object sender, RoutedEventArgs e)
        {
            TrackedTaskListWindow listWindow = new TrackedTaskListWindow();
            TrackerViewModel model = (TrackerViewModel)this.DataContext;
            listWindow.DataContext = model.TrackedTimeRanges;
            listWindow.Show();
        }

        private void SyncBtn_Click(object sender, RoutedEventArgs e)
        {
            TrackerViewModel model = (TrackerViewModel)this.DataContext;
            model.ReloadQiTasks();
            model.SaveTimeRangesToQi();
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            TrackerViewModel model = (TrackerViewModel)this.DataContext;
            model.Exit();
        }
    }
}
