using GongSolutions.Wpf.DragDrop;
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
    public class RecycleBinViewModel : IDropTarget, INotifyPropertyChanged
    {
        private bool _drgTskCanBeRemoved = false;
        public bool DraggedTaskCanBeRemoved
        {
            get
            {
                return _drgTskCanBeRemoved;
            }
            set
            {
                _drgTskCanBeRemoved = value;
                OnPropertyChanged();
            }
        }

        TrackerViewModel tracker;
        public event PropertyChangedEventHandler PropertyChanged;

        public RecycleBinViewModel(TrackerViewModel tracker)
        {
            this.tracker = tracker;
        }

        public void DragEnter(IDropInfo dropInfo)
        {
        }

        public void DragLeave(IDropInfo dropInfo)
        {
           
        }

        public void DragOver(IDropInfo dropInfo)
        {
            if (!(dropInfo.Data is NewTask))
            {
                dropInfo.Effects = System.Windows.DragDropEffects.None;
            }
            else
            {
                dropInfo.Effects = System.Windows.DragDropEffects.Move;
            }
        }

        public void Drop(IDropInfo dropInfo)
        {
            
            tracker.Tasks.Remove((ITask)dropInfo.Data);
            tracker.FilterTasks();
            DraggedTaskCanBeRemoved = false;
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
