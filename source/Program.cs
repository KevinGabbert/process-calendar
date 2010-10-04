using System;
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

        static void Main(string[] args)
        {
            Console.WriteLine("Process Calendar v10.03.10" + DateTime.Now.ToShortTimeString());

            var getUserInfo = new Login_Form();

            getUserInfo.ShowDialog();  //Code Execution will stop at this point and wait until user has dismissed the Login form.

            string userName = getUserInfo.User; 
            string password = getUserInfo.Password;
            _calendarToPost = getUserInfo.PostURI;

            var toDo = new CalendarLogManager();

            var processes = from p in XElement.Load(@"..\..\ProcessToLog.xml").Elements("Process")
                            select p;

            while (true)
            {
                Process[] processlist = Process.GetProcesses();
                
                Program.Reset(toDo);
                Program.AddOrUpdate_Matching_Processes(toDo, processlist, processes);
                Program.Log_Stopped_Processes(toDo, userName, password);

                Console.WriteLine("Sleeping for 1 minute");
                Thread.Sleep(new TimeSpan(0,0,1,0));   
            }
        }

        private static void Log_Stopped_Processes(CalendarLogManager toDo, string userName, string password)
        {
            foreach (RecordedProcess recordedProcess in toDo.Where(process => (!process.StillRunning) && (process.StartTime != new DateTime())))
            {
                Console.WriteLine(recordedProcess.ProcessName + " presumed stopped. Logging to Calendar");
                Program.CreateEntry(userName, password, recordedProcess.ProcessName, recordedProcess.MainWindowTitle, recordedProcess.StartTime, DateTime.Now);
                Console.WriteLine(recordedProcess.ProcessName + " activity logged " + DateTime.Now.ToShortTimeString());

                toDo.Remove(recordedProcess);
                break; //remove the rest in the next go around..
            }
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
            foreach (Process processItem in processlist)
            {
                if (processItem != null)
                {
                    foreach (XElement element in processes.Where(element => element.Value == processItem.ProcessName))
                    {
                        Program.LogDetect(processItem);
                        toDo.AddOrUpdate(processItem, true);
                    }
                }
            }
        }

        private static void LogDetect(Process processItem)
        {
            Console.WriteLine(processItem.ProcessName + DETECTED_RUNNING + DateTime.Now.ToShortTimeString());
        }
        private static readonly Google.GData.Calendar.CalendarService _service = new CalendarService("My Google Calendar Service");
    
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

                GDataGAuthRequestFactory requestFactory = (GDataGAuthRequestFactory)_service.RequestFactory;
                requestFactory.CreateRequest(GDataRequestType.Insert, _calendarToPost);

                //(new GDataGAuthRequestFactory("", "")).CreateRequest(GDataRequestType.Insert, postUri);

                // Send the request and receive the response:
                _service.Insert(_calendarToPost, entry);

                //insertedEntry.Published

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
            CalendarService myService = new CalendarService("exampleCo-exampleApp-1");
            myService.setUserCredentials(userName, password);

            CalendarQuery query = new CalendarQuery();
            query.Uri = new Uri("http://www.google.com/calendar/feeds/default/owncalendars/full");
            CalendarFeed resultFeed = myService.Query(query);
            return resultFeed;
        }
    }
}


                    //CalendarFeed cal_Feed = RetrievingOwnGoogleCalendars();
                    //foreach (CalendarEntry centry in cal_Feed.Entries)
                    //{
                    //    cmbGoogleCalendar.Items.Add(centry);
                    //}