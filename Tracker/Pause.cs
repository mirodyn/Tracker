using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tracker
{
    public class Pause : BaseTask
    {
        public Pause() : base()
        {
            QiCode = "pause";
            Name = "Pauza";
        }

        public Pause(string code) : base(code, "Pauza")
        {
        }
    }
}
