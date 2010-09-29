using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ProcessCalendar
{
    public class ProcessToRecord
    {
        public int Id { get; set; }
        public string ProcessName { get; set;}
        public bool StillRunning { get; set; }
        public DateTime StartTime { get; set; }
    }
}
