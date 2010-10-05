using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace ProcessCalendar
{
    public class CalendarLogManager: List<RecordedProcess>
    {
        public void Add(RecordedProcess process, bool stillRunning)
        {
            base.Add(process);  
        }

        public void AddOrUpdate(Process process, bool stillRunning)
        {
            try
            {
                int i = 0;
                foreach (RecordedProcess entry in this.Where(entry => entry.Id == process.Id))
                {
                    i++;
                    entry.StartTime = process.StartTime;
                    entry.StillRunning = stillRunning;
                    break;
                }

                if (i == 0)
                {
                    var item = new RecordedProcess();

                    item.Id = process.Id;
                    item.ProcessName = process.ProcessName;
                    item.StartTime = process.StartTime;
                    item.StillRunning = stillRunning;

                    item.MainWindowTitle = process.MainWindowTitle.ToString();

                    base.Add(item);
                }

            }
            catch (InvalidOperationException ix)
            {
                
            }
        }
    }
}
