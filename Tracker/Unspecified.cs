using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tracker
{
    public class Unspecified : BaseTask
    {

        public Unspecified() : base() 
        {
            Name = "unspecified task";
        }
       
        public Unspecified(string code) : base(code,"new task")
        {
        }
    }
}
