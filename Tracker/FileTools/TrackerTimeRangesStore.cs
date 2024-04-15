using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Tracker.FileTools
{
    public class TrackerTimeRangesStore
    {
        public DateTime Day { get; set; }
        public bool Archive { get; set; }
        public DateTime SaveTime { get; set; }
        public List<TaskTimeRange> TimeRanges { get; set; }

        private static string trackerDataPath = Path.Combine(Environment.GetEnvironmentVariable("LocalAppData"),"QiTracker","tracker-data");

        public TrackerTimeRangesStore(IEnumerable<TaskTimeRange> ranges,DateTime day)
        {
            //SaveTime = DateTime.Now;
            Archive = false;
            Day = day.Date;
            TimeRanges = ranges.ToList();
        }

        public TrackerTimeRangesStore(DateTime day)
        {
            //SaveTime = DateTime.Now;
            Archive = false;
            Day = day.Date;
            TimeRanges = new List<TaskTimeRange>();
        }
        public TrackerTimeRangesStore()
        {
            //SaveTime = DateTime.Now;
            Archive = false;
            Day = DateTime.MinValue.Date;
            TimeRanges = new List<TaskTimeRange>();
        }

        public void Save()
        {
            SaveTime = DateTime.Now;
            string name = Day.ToString("yyyy-MM-dd") + "-tracked.json";
            string tmpName = "tmp-" + name;
            name = Path.Combine(GetRelativeFolder(), name);
            tmpName = Path.Combine(GetRelativeFolder(), tmpName);

            if (File.Exists(name)) File.Copy(name, tmpName);

            string json = JsonSerializer.Serialize<TrackerTimeRangesStore>(this);
            File.WriteAllText(name, json);

            if (File.Exists(tmpName)) File.Delete(tmpName);
        }

        public static TrackerTimeRangesStore LoadTodaysTimeRanges()
        {
            return LoadTimeRangesByDate(DateTime.Today);
        }

        public static TrackerTimeRangesStore LoadTimeRangesByDate(DateTime date) 
        {
            string name = date.ToString("yyyy-MM-dd") + "-tracked.json";
            name = Path.Combine(GetRelativeFolder(), name);
            if (!File.Exists(name)) return new TrackerTimeRangesStore(date);
            return JsonSerializer.Deserialize<TrackerTimeRangesStore>(File.ReadAllText(name));
        }

        public static IEnumerable<TrackerTimeRangesStore> LoadAllTimeRanges()
        {
            List<TrackerTimeRangesStore> res = new List<TrackerTimeRangesStore>();
            foreach (DirectoryInfo di in new DirectoryInfo(trackerDataPath).EnumerateDirectories())
            {
                foreach (FileInfo file in di.GetFiles())
                {
                    res.Add(JsonSerializer.Deserialize<TrackerTimeRangesStore>(File.ReadAllText(file.FullName)));
                }
            }
            return res;
        }

        private static string GetRelativeFolder()
        {
            string path = Path.Combine(trackerDataPath, DateTime.Today.ToString("yyyy-MM"));
            if (!Directory.Exists(path)) Directory.CreateDirectory(path);
            return path;
        }


    }
}
