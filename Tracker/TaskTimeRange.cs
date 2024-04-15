using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;

namespace Tracker
{
    public class TaskTimeRange
    {

        [JsonIgnore]
        public ITask LinkedTask { get; set; }
        public string QiCode { get; set; }
        public DateTime From { get; set; }
        public DateTime To { get; set; }
        public bool SavedInQi {get;set;}
        public string Annotation { get; set; }

        public string TimeRangeGuid { get; set; }

        public TaskTimeRange()
        {
            TimeRangeGuid = Guid.NewGuid().ToString();
            From = DateTime.Now;
            SavedInQi = false;
        }

        public void StopNow()
        {
            To= DateTime.Now;
        }

        public void AttachTask(IEnumerable<ITask> tasks)
        {
            try
            {
                if(tasks != null && tasks.Count() > 0 ) LinkedTask = tasks.Where<ITask>(x => x.QiCode == QiCode)?.First();
            }catch(InvalidOperationException e)
            {
                //linked task probably inst in the collection... 
            }

        }
    }
}