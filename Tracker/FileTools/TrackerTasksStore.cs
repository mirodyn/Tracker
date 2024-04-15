using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Tracker.FileTools
{
    class TrackerTasksStore
    {
        public DateTime SaveTime { get; set; }
        public List<NewTask> NewTasks { get; set; }
        public List<Task> QiTasks { get; set; }

        private static string trackerDataPath = Path.Combine(Environment.GetEnvironmentVariable("LocalAppData"), "QiTracker", "tracker-data");

        public TrackerTasksStore() 
        {
            NewTasks = new List<NewTask>();
            QiTasks = new List<Task>();
        }
        public TrackerTasksStore(IEnumerable<ITask> tasks)
        {   
            NewTasks = new List<NewTask>();
            QiTasks = new List<Task>();
            if (tasks == null || tasks.Count() == 0) return;
            NewTasks = tasks.Where(x => x is NewTask).Cast<NewTask>().ToList();
            QiTasks = tasks.Where(x => x is Task).Cast<Task>().ToList();
        }


        public void Save()
        {
            SaveTime = DateTime.Now;
            string name = "tracker-tasks.json";
            string tmpName = "tmp-" + name;
            name = Path.Combine(GetTempFolder(), name);
            tmpName = Path.Combine(GetTempFolder(), tmpName);

            if (File.Exists(name)) File.Copy(name, tmpName);

            string json = JsonSerializer.Serialize<TrackerTasksStore>(this);
            File.WriteAllText(name, json);

            if (File.Exists(tmpName)) File.Delete(tmpName);
        }

        public static TrackerTasksStore LoadTasks()
        {
            string name = "tracker-tasks.json";
            name = Path.Combine(GetTempFolder(), name);
            if (!File.Exists(name)) return new TrackerTasksStore();
            return JsonSerializer.Deserialize<TrackerTasksStore>(File.ReadAllText(name));
        }

        private static string GetTempFolder()
        {
            
            if (!Directory.Exists(trackerDataPath)) Directory.CreateDirectory(trackerDataPath);
            return trackerDataPath;
        }


    }
}
