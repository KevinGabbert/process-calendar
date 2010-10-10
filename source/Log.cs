using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Xml.Linq;

namespace ProcessCalendar
{
    class Log
    {
        public const string DETECTED_RUNNING = " detected running ";
        public static Uri _calendarToPost = new Uri("http://www.google.com/calendar/feeds/default/private/full"); //default string. probably not even needed, but its helpful to know the format.

        public static string LogVersion { get; set; }

        public static void Log_Watched_Processes(IEnumerable<XElement> processesToWatch, string userName, string password, Uri logToCalendar)
        {
            var toDo = new CalendarLogManager();
            _calendarToPost = logToCalendar;

            while (true) //ToDo: Time limit should be set in XML
            {
                Process[] processlist = Process.GetProcesses();

                Log.Reset(toDo);
                Log.AddOrUpdate_Matching_Processes(toDo, processlist, processesToWatch);

                IList<RecordedProcess> presumedStoppedProcesses = (from processItem in toDo.Where(element => element.StillRunning == false)
                                                                   select processItem).Where(processItem => processItem.StartTime != new DateTime()).ToList();

                for (int i = presumedStoppedProcesses.Count() - 1; i > 0; i--) //Foreach won't work here.
                {
                    RecordedProcess current = presumedStoppedProcesses[i];

                    Log.Log_Stopped_Process(current, userName, password);
                    toDo.Remove(current);
                }

                Console.WriteLine("Sleeping for 1 minute");
                Thread.Sleep(new TimeSpan(0, 0, 1, 0));
            }
        }
        private static void Log_Stopped_Process(RecordedProcess recordedProcess, string userName, string password)
        {
            Console.WriteLine(recordedProcess.ProcessName + " presumed stopped. Logging to Calendar: " + _calendarToPost.OriginalString);
            GoogleCalendar.CreateEntry(userName, password, recordedProcess.ProcessName, recordedProcess.MainWindowTitle + ",  " + Log.LogVersion, recordedProcess.StartTime, DateTime.Now);
            Console.WriteLine(recordedProcess.ProcessName + " activity logged " + DateTime.Now.ToShortTimeString());
        }
        private static void Reset(IEnumerable<RecordedProcess> toDo)
        {
            //we no longer know if they are still running.
            foreach (RecordedProcess process in toDo)
            {
                process.StillRunning = false;
            }
        }
        private static void AddOrUpdate_Matching_Processes(CalendarLogManager toDo, IEnumerable<Process> processlist, IEnumerable<XElement> processes)
        {
            Console.WriteLine("Start " + DateTime.Now.ToShortTimeString());
            foreach (Process processItem in from processItem in processlist
                                            where processItem != null
                                            from element in processes.Where(element => element.Value == processItem.ProcessName)
                                            select processItem)
            {
                Log.LogDetect(processItem);
                toDo.AddOrUpdate(processItem, true);
            }
        }
        private static void LogDetect(Process processItem)
        {
            Console.WriteLine(processItem.ProcessName + DETECTED_RUNNING + DateTime.Now.ToShortTimeString());
        }
    }
}
