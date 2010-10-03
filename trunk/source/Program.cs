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

        static void Main(string[] args)
        {
            Console.WriteLine("Process Calendar v10.03.10" + DateTime.Now.ToShortTimeString());

            const string userName = "euiei";
            const string password = "euiueieui";

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
                Program.CreateEntry(userName, password, recordedProcess.MainWindowTitle, "", recordedProcess.StartTime, DateTime.Now);
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

                //hecate-d
                Uri postUri = new Uri("http://www.google.com/calendar/feeds/fq4am35og85t9qenbpn99gfk80%40group.calendar.google.com/private/full");
                          //IHAPPYDAY "http://www.google.com/calendar/feeds/einraj3jdi67j9qf3crq8bqbhc%40group.calendar.google.com/private/full"
                
                GDataGAuthRequestFactory requestFactory = (GDataGAuthRequestFactory)_service.RequestFactory;

                requestFactory.CreateRequest(GDataRequestType.Insert, postUri);

                // Send the request and receive the response:
                _service.Insert(postUri, entry);

                //insertedEntry.Published

                Console.WriteLine("Event Successfully Added");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message.ToString());
            }
        }
    }
}


                    //CalendarFeed cal_Feed = RetrievingOwnGoogleCalendars();
                    //foreach (CalendarEntry centry in cal_Feed.Entries)
                    //{
                    //    cmbGoogleCalendar.Items.Add(centry);
                    //}