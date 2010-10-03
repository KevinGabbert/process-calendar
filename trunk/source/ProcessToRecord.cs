using System;

namespace ProcessCalendar
{
    public class RecordedProcess
    {
        public int Id { get; set; }
        public string ProcessName { get; set;}
        public bool StillRunning { get; set; }
        public DateTime StartTime { get; set; }
        public string MainWindowTitle { get; set; }
    }
}
