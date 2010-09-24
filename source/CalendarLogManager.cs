using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ProcessCalendar
{
    public class CalendarLogManager: List<ProcessToRecord>
    {
        public void Add(ProcessToRecord process, bool stillRunning)
        {
            base.Add(process);
        }

        public void AddOrUpdate(ProcessToRecord process, bool stillRunning)
        {
            //look up by ID. (Use Linq)
            ProcessToRecord item = this.Single(s => s == process);

            //-can't find? create a new process

            //If null then keep going

            //- CAN find? Update the one in there
            //- this[x] = process as ProcessToRecord;
            //- this[x].StillRunning = stillRunning
            item = process; //overwrite what was there
            item.StillRunning = stillRunning;


            //base.Add(process);
        }

        public int StilRunning { get; set; }
    }
}
