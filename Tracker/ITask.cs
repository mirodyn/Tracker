using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Security.RightsManagement;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Tracker
{
    public interface ITask
    {
        string Name { get; set; } 
        string QiCode { get; set; }
        int ActivityIndex { get; set; }
        [JsonIgnore]
        ObservableCollection<TaskTimeRange> TimeRanges { get; set; } 

    }
}
