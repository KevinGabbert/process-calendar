using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ProcessCalendar
{
    public class ProcessToRecord: System.Diagnostics.Process
    {
        public bool StillRunning { get; set; }
    }
}
