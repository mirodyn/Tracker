using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing.Text;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tracker.FileTools;

namespace Tracker.TrackedTasksList
{
    public class TrackedDayViewModel
    {
        public ObservableCollection<TaskTimeRange> TimeRanges { get; set; }

        private TrackerTimeRangesStore storedDay;

        public String Day { get; }
        private DateTime _day;


        public TrackedDayViewModel()
        {
            Day = DateTime.Today.ToString("dd-MM-yyyy");
            _day = DateTime.Today;
            TimeRanges = new ObservableCollection<TaskTimeRange>();
        }

        public TrackedDayViewModel(TrackerTimeRangesStore day, IEnumerable<ITask> tasks) 
        {
            TimeRanges = new ObservableCollection<TaskTimeRange>();
            storedDay = day;
            Day = day.Day.ToString("dd-MM-yyyy");
            _day = day.Day;
            foreach (TaskTimeRange timeRange in day.TimeRanges)
            {
                TimeRanges.Add(timeRange);
            }
            AttachTasks(tasks);
        }

        private void AttachTasks(IEnumerable<ITask> tasks)
        {
            foreach (TaskTimeRange timeRange in TimeRanges)
            {
                timeRange.AttachTask(tasks);
            }
        }

        public void SaveDay()
        {
            TrackerTimeRangesStore ttbd = new TrackerTimeRangesStore(TimeRanges,_day);
            ttbd.Save();
        }

        public IEnumerable<TaskTimeRange> GetUnsynchronizedTimeRanges()
        {
            return TimeRanges.Where(t => t.SavedInQi == false);
        }
    }
}
