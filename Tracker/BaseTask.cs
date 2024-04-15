using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Tracker
{
    public class BaseTask : ITask, INotifyPropertyChanged
    {
        public int ActivityIndex { get; set; }
        public string QiCode { get; set; }
        public string Name { get; set; }

        public ObservableCollection<TaskTimeRange> TimeRanges {get;set;}


        public event PropertyChangedEventHandler PropertyChanged;


        public BaseTask()
        {
            QiCode = Guid.NewGuid().ToString();
            Name = "unknown";
            TimeRanges= new ObservableCollection<TaskTimeRange>();
        }

        public BaseTask(string qiCode, string description)
        {
            QiCode = qiCode;
            Name = description;
            TimeRanges= new ObservableCollection<TaskTimeRange>();
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
