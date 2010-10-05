using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Xml.Linq;
using Google.GData.Calendar;
using Google.GData.Client;
using Google.GData.Extensions;

namespace ProcessCalendar
{
    class Program
    {
        public const string DETECTED_RUNNING = " detected running ";
        public static Uri _calendarToPost = new Uri("http://www.google.com/calendar/feeds/default/private/full");
        private static readonly Google.GData.Calendar.CalendarService _service = new CalendarService("processLogService");
        public const string VERSION = "Process Calendar v10.04.10 ";


        static void Main(string[] args)
        {
            Console.WriteLine(VERSION + DateTime.Now.ToShortTimeString());

            var userInfoForm = new Login_Form();
            userInfoForm.ShowDialog();  
            
            //*** Code Execution will stop at this point and wait until user has dismissed the Login form. ***//

            _calendarToPost = userInfoForm.PostURI;
            Program.LogWatchedProcesses(Program.GetWatchList(), userInfoForm.User, userInfoForm.Password);
        }

        private static IEnumerable<XElement> GetWatchList()
        {
            return from p in XElement.Load(@"..\..\ProcessToLog.xml").Elements("Process")
                   select p;
        }

        private static void LogWatchedProcesses(IEnumerable<XElement> processesToWatch, string userName, string password)
        {
            var toDo = new CalendarLogManager();
            while (true) //ToDo: Time limit should be set in XML
            {
                Process[] processlist = Process.GetProcesses();
                
                Program.Reset(toDo);
                Program.AddOrUpdate_Matching_Processes(toDo, processlist, processesToWatch);

                IList<RecordedProcess> presumedStoppedProcesses = (from processItem in toDo.Where(element => element.StillRunning == false)
                                                                   select processItem).Where(processItem => processItem.StartTime != new DateTime()).ToList();

                for (int i = presumedStoppedProcesses.Count() - 1; i > 0; i--) 
                {
                    RecordedProcess current = presumedStoppedProcesses[i];

                    Program.Log_Stopped_Process(current, userName, password);
                    toDo.Remove(current);
                }

                Console.WriteLine("Sleeping for 1 minute");
                Thread.Sleep(new TimeSpan(0,0,1,0));   
            }
        }

        private static void Log_Stopped_Process(RecordedProcess recordedProcess, string userName, string password)
        {
            Console.WriteLine(recordedProcess.ProcessName + " presumed stopped. Logging to Calendar: " + _calendarToPost.OriginalString);
            Program.CreateEntry(userName, password, recordedProcess.ProcessName, recordedProcess.MainWindowTitle + ",  " + VERSION, recordedProcess.StartTime, DateTime.Now);
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
                Program.LogDetect(processItem);
                toDo.AddOrUpdate(processItem, true);
            }
        }

        private static void LogDetect(Process processItem)
        {
            Console.WriteLine(processItem.ProcessName + DETECTED_RUNNING + DateTime.Now.ToShortTimeString());
        } 
        public static void CreateEntry(string userName, string password, string title, string description, DateTime start, DateTime end)
        {
            try
            {
                var entry = new EventEntry { Title = { Text = title }, Content = { Content = description } };

                // Set a location for the event.
                Where eventLocation = new Where { ValueString = "auto-logger" };
                entry.Locations.Add(eventLocation);

                When eventTime = new When(start, end);
                entry.Times.Add(eventTime);

                if (!string.IsNullOrEmpty(userName))
                {
                    _service.setUserCredentials(userName, password);
                }

                (new GDataGAuthRequestFactory("", "")).CreateRequest(GDataRequestType.Insert, _calendarToPost);
                _service.Insert(_calendarToPost, entry);

                Console.WriteLine("Event Successfully Added");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message.ToString());
            }
        }

        public static CalendarFeed RetrievingOwnGoogleCalendars(string userName, string password)
        {
            // Create a CalenderService and authenticate
            var myService = new CalendarService("process-calendar");
            myService.setUserCredentials(userName, password);

            var query = new CalendarQuery();
            query.Uri = new Uri("http://www.google.com/calendar/feeds/default/owncalendars/full");
            CalendarFeed resultFeed = myService.Query(query);
            return resultFeed;
        }
    }
}