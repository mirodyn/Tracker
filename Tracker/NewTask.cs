using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace Tracker
{
    public class NewTask : BaseTask
    {
        public NewTask(string description) : base(Guid.NewGuid().ToString(), description)
        {
        }

        public NewTask() :base() { }

    }
}
