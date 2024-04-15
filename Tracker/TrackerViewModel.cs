using GongSolutions.Wpf.DragDrop;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Configuration;
using System.Linq;
using System.Net.NetworkInformation;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Markup;
using System.Windows.Media.TextFormatting;
using System.Windows.Threading;
using Tracker.FileTools;
using Tracker.QiTools;
using Tracker.TrackedTasksList;

namespace Tracker
{
    public class TrackerViewModel : IDropTarget, IDragSource, INotifyPropertyChanged
    {
        private string _filterString = "";
        public string FilterString { 
            get { return _filterString; } 
            set 
            { 
                _filterString = value;
                FilterTasks();
            } 
        }
        public string CurrentAnnotation { get; set; }
        public ObservableCollection<ITask> Tasks { get; set; }
        public ObservableCollection<ITask> FilteredTasks { get; set; }
        public ObservableCollection<ITask> SpecialTasks { get; set; }

        //todo: Tato property je udelana jako ObservableCollection, ale mohla a asi i mela, by byt
        //jako jednoducha property typu ITask. 
        //Nejak mi lae pri vyvoji zlobil binding z view, tak jsem to nechal takto.
        public ObservableCollection<ITask> ActiveTask { get; set; }

        public Config Configuration { get; set; }

        private bool _qiReachable;
        public bool QiReachable {
            get { return _qiReachable; }

            set 
            {
                _qiReachable = value;
                OnPropertyChanged();
            }
        }

        //public ObservableCollection<TaskTimeRange> TaskTimeRanges { get; set; } 

        public TrackedTaskListViewModel TrackedTimeRanges { get; set; }

        private TaskTimeRange currentTimeRange; 
        
        private int newTrackerTaskIndex = 1;
        public RecycleBinViewModel Bin { get; set; }


        private DispatcherTimer _qiPingTimer;

        public event PropertyChangedEventHandler PropertyChanged;

        public TrackerViewModel()
        {
            //TaskTimeRanges = new ObservableCollection<TaskTimeRange>();
            ActiveTask = new ObservableCollection<ITask>();
            Tasks = new ObservableCollection<ITask>();
            Bin = new RecycleBinViewModel(this);
            SpecialTasks = new ObservableCollection<ITask>();
            SpecialTasks.Add(new Pause());
            SpecialTasks.Add(new Unspecified(Guid.NewGuid().ToString()));
            QiReachable = false;
            Configuration = Config.LoadFromFile();
            FilteredTasks = new ObservableCollection<ITask>();
            ReloadQiTasks();
            ReloadTimeRanges();
            StartQiPingTimer();

        }

        private void ReloadTimeRanges()
        {
            TrackedTimeRanges = new TrackedTaskListViewModel(TrackerTimeRangesStore.LoadAllTimeRanges(),Tasks);
        }

        private void StartQiPingTimer()
        {
            string qiadd = Configuration.QiConfig.Server;
            _qiPingTimer = new DispatcherTimer(new TimeSpan(0, 1, 0), DispatcherPriority.Normal, delegate
            {
                QiReachable = PingHost(qiadd);
            }, Application.Current.Dispatcher);

            _qiPingTimer.Start();
        }

        private void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        public void ReloadQiTasks()
        {
            TrackerTasksStore tts = TrackerTasksStore.LoadTasks();
            foreach (ITask task in tts.QiTasks)
            {
                if (Tasks.ToList<ITask>().Where(t => t.QiCode == task.QiCode).Count() == 0) Tasks.Add(task);
            }
            foreach (ITask task in tts.NewTasks)
            {
                if (Tasks.ToList<ITask>().Where(t => t.QiCode == task.QiCode).Count() == 0) Tasks.Add(task);
            }

            if (Configuration.ConfigReady && PingHost(Configuration.QiConfig.Server))
            {
                QiReachable = true;
                //int index = 0;
                foreach (string[] tsk in LoadFromQi())
                {
                    if (Tasks.ToList<ITask>().Where(t => t.QiCode == tsk[1]).Count() == 0 )
                    {
                        Task t = new Task(tsk[1], tsk[0], tsk[2], tsk[3]);
                        //t.ActivityIndex = index;
                        t.ActivityIndex = 0;
                        Tasks.Add(t);
                        //index++;
                    }
                }
            }
            tts = new TrackerTasksStore(Tasks);
            tts.Save();
            FilterTasks();
        }
        internal void FilterTasks()
        {
            //reorder tasks
            IOrderedEnumerable<ITask> ordered = Tasks.OrderByDescending(t => t.ActivityIndex);
            List<ITask> tmp = ordered.ToList();
            Tasks.Clear();
            foreach(ITask t in tmp)
            {
                Tasks.Add(t);
            } 
            //apply filters
            FilteredTasks.Clear();
            string filter = FilterString.ToLower();
            foreach(ITask tsk in Tasks)
            {
                string customer = "";
                if (tsk is Task ) customer = ((Task)tsk).Customer?.ToLower();
                string code = tsk.QiCode?.ToLower();
                string name = tsk.Name?.ToLower();
                if (String.IsNullOrEmpty(filter)
                    || ((!String.IsNullOrEmpty(customer) && customer.Contains(filter))
                    ||(!String.IsNullOrEmpty(code) && code.Contains(filter)) 
                    ||(!String.IsNullOrEmpty(name) && name.Contains(filter)))) FilteredTasks.Add(tsk);
            }
            
        }

        public void SaveTimeRangesToQi()
        {
            if (Configuration.ConfigReady && PingHost(Configuration.QiConfig.Server))
            {
                QiDataAccess qiWriter = new QiDataAccess(Configuration.QiConfig);
                qiWriter.LogIn();
                foreach (TaskTimeRange t in TrackedTimeRanges.GetAllUnsynchronizedTimeRanges())
                {
                    if (t.LinkedTask is NewTask)
                    {
                        t.SavedInQi = qiWriter.WriteNewTaskTimeRangeToQi(t, Configuration);
                    }
                    else
                    {
                        t.SavedInQi = qiWriter.WriteTaskTimeRangeToQi(t, Configuration);
                    }
                }
                qiWriter.LogOut();

                TrackedTimeRanges.SaveAllDays();

            }
        }

        public void Exit()
        {
            FinishCurrentTask();
            TrackerTasksStore tts = new TrackerTasksStore(Tasks);
            tts.Save();
            TrackedTimeRanges.SaveAllDays();
            
        }

        public void FinishCurrentTask()
        {
            if (ActiveTask.Count != 1) return;
            if (ActiveTask[0] is Task) FinishTimeRange();
            if (ActiveTask[0] is NewTask) FinishNewTask();
            if (Tasks.Count > 0) ActiveTask[0].ActivityIndex = Tasks[0].ActivityIndex + 1;

            //TrackerTimeRangesStore ttbd = new TrackerTimeRangesStore(TaskTimeRanges);
            //ttbd.Save();

            ActiveTask.Clear();
        }

        private void FinishNewTask()
        {
            if (!Tasks.Contains(ActiveTask[0])) Tasks.Add(ActiveTask[0]);
            FinishTimeRange();
        }

        private void FinishTimeRange()
        {
            currentTimeRange.StopNow();
            currentTimeRange.Annotation = CurrentAnnotation;
            CurrentAnnotation = "";
            currentTimeRange.LinkedTask = ActiveTask[0];
            currentTimeRange.QiCode = ActiveTask[0].QiCode;
            if ((currentTimeRange.To - currentTimeRange.From).TotalMinutes >= 1) 
            {
                TrackedTimeRanges.Add(currentTimeRange);
                //TaskTimeRanges.Add(currentTimeRange);
                //ActiveTask[0].TimeRanges.Add(currentTimeRange);
            }
            currentTimeRange = null;
        }

        private List<string[]> LoadFromQi()
        {
            List<string[]> list = new List<string[]>();
            try
            {
                QiDataAccess qiReader = new QiDataAccess(Configuration.QiConfig);
                qiReader.LogIn();
                list = qiReader.ReadTasksFromDataset();
                qiReader.LogOut();
            }
            catch
            {
                //nothing
            }
            return list;

        }

        public void DragOver(IDropInfo dropInfo)
        {
            dropInfo.Effects = DragDropEffects.Move;
            return;
        }

        public void Drop(IDropInfo dropInfo)
        {
            FinishCurrentTask();
            //ReorderTasks();
            FilterTasks();
            currentTimeRange = new TaskTimeRange();
            ActiveTask.Add((ITask)dropInfo.Data);
            Bin.DraggedTaskCanBeRemoved = false;
        }


        public void StartDrag(IDragInfo dragInfo)
        {
            if (dragInfo.SourceItem is Task || dragInfo.SourceItem is Pause || dragInfo.SourceItem is NewTask)
            {
                Bin.DraggedTaskCanBeRemoved = false;
                dragInfo.Data = dragInfo.SourceItem;
                dragInfo.Effects = DragDropEffects.Move;
                if (dragInfo.SourceItem is NewTask) Bin.DraggedTaskCanBeRemoved = true;
            }
          
            if (dragInfo.SourceItem is Unspecified)
            {
                dragInfo.Data = new NewTask("New tracker task " + newTrackerTaskIndex.ToString());
                newTrackerTaskIndex++;
                dragInfo.Effects = DragDropEffects.Move;
            }
        }

        public bool CanStartDrag(IDragInfo dragInfo)
        {
            return true;
        }

        public void Dropped(IDropInfo dropInfo)
        {
        }

        public void DragDropOperationFinished(DragDropEffects operationResult, IDragInfo dragInfo)
        {
        }

        public void DragCancelled()
        {
            Bin.DraggedTaskCanBeRemoved = false;
        }

        public bool TryCatchOccurredException(Exception exception)
        {
            return false;
        }


        private bool PingHost(string nameOrAddress)
        {
            bool pingable = false;
            Ping pinger = null;

            try
            {
                pinger = new Ping();
                PingReply reply = pinger.Send(nameOrAddress);
                pingable = reply.Status == IPStatus.Success;
            }
            catch (PingException)
            {
                // Discard PingExceptions and return false;
            }
            finally
            {
                if (pinger != null)
                {
                    pinger.Dispose();
                }
            }

            return pingable;
        }

        public void DragEnter(IDropInfo dropInfo)
        {
           
        }

        public void DragLeave(IDropInfo dropInfo)
        {
           
        }
    }
}
