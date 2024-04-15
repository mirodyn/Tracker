using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Tracker.FileTools;

namespace Tracker.TrackedTasksList
{
    public class TrackedTaskListViewModel : INotifyPropertyChanged
    {
        public TrackedDayViewModel Today { get; set; }
        public ObservableCollection<TrackedDayViewModel> UnsynchronizedDays { get; set; }
        public ObservableCollection<TrackedDayViewModel> ArchivedDays { get; set; }

        private TrackedDayViewModel _selectedDay;

        public event PropertyChangedEventHandler PropertyChanged;

        public TrackedDayViewModel SelectedDay { 
            get
            {
                return _selectedDay;
            }
            set
            {
                _selectedDay = value;
                OnPropertyChanged();

            }
        }

        public TrackedTaskListViewModel(IEnumerable<TrackerTimeRangesStore> days, IEnumerable<ITask> tasks)
        {
            UnsynchronizedDays = new ObservableCollection<TrackedDayViewModel>();
            ArchivedDays = new ObservableCollection<TrackedDayViewModel>();
            foreach (TrackerTimeRangesStore day in days)
            {
                if (day.Day == DateTime.Today)
                {
                    Today = new TrackedDayViewModel(day,tasks);
                }
                else if (day.Archive)
                {
                    ArchivedDays.Add(new TrackedDayViewModel(day,tasks));
                }
                else
                {
                    UnsynchronizedDays.Add(new TrackedDayViewModel(day,tasks));
                }
            }
            if (Today == null) Today = new TrackedDayViewModel();
            SelectedDay = Today;
        }

        public void SaveAllDays()
        {
            Today.SaveDay();
            foreach (TrackedDayViewModel day in UnsynchronizedDays)
            {
                day.SaveDay();
            }
        }
        public void Add(TaskTimeRange range)
        {
            Today.TimeRanges.Add(range);
            Today.SaveDay();
        }

        public IEnumerable<TaskTimeRange> GetAllUnsynchronizedTimeRanges()
        {
            List<TaskTimeRange> result = new List<TaskTimeRange>();
            result.AddRange(Today.GetUnsynchronizedTimeRanges());
            foreach (TrackedDayViewModel day in UnsynchronizedDays)
            {
                result.AddRange(day.GetUnsynchronizedTimeRanges());
            }
            return result;
        }
        private void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
