using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Tracker
{
    public class Task : BaseTask, INotifyPropertyChanged
    {
        public string Customer { get; set; }
        public string Icu { get; set; }
        //public List<TaskTimeRange> TimeRanges { get; set; } 

        public event PropertyChangedEventHandler PropertyChanged;


        public Task(): base()
        {
            Name = "unknown task";
        }

        public Task(string code, string desc,string customer,string icu) : base(code,desc)
        {
            //TimeRanges = new List<TaskTimeRange>();
            Customer = customer;
            Icu = icu;
        }


    }

}
