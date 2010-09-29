using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace ProcessCalendar
{
    public class CalendarLogManager: List<ProcessToRecord>
    {
        public void Add(ProcessToRecord process, bool stillRunning)
        {
            base.Add(process);
        }

        public void AddOrUpdate(Process process, bool stillRunning)
        {
            try
            {
                int i = 0;
                foreach (ProcessToRecord entry in this.Where(entry => entry.Id == process.Id))
                {
                    i++;
                    entry.StillRunning = stillRunning;
                    //StartTime = new DateTime();
                    break;
                }

                if (i == 0)
                {
                    ProcessToRecord item = new ProcessToRecord();

                    item.Id = process.Id;
                    item.ProcessName = process.ProcessName;
                    item.StartTime = process.StartTime;
                    item.StillRunning = stillRunning;

                    base.Add(item);
                }

            }
            catch (InvalidOperationException ix)
            {
                
            }
        }

        public int StilRunning { get; set; }
    }
}
