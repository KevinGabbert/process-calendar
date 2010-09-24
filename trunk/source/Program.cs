using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using Google.GData.Calendar;
using Google.GData.Client;
using Google.GData.Extensions;
using System.Net; //for browser windows..

namespace ProcessCalendar
{
    class Program
    {
        public const string DETECTED_RUNNING = " detected running ";

        static void Main(string[] args)
        {
            Console.WriteLine("Process Calendar v9.23.10b" + DateTime.Now.ToShortTimeString());

            const string userName = "hhhhhhh";
            const string password = "hdhdhdtd";

            DateTime sketchupStartTime = new DateTime();
            DateTime devEnvStartTime = new DateTime();
            DateTime chromeStartTime = new DateTime();
            DateTime firefoxStartTime = new DateTime();

            var toDo = new CalendarLogManager();

            while (true)
            {
                bool sketchupStillRunning = false;
                bool devEnvStillRunning = false;
                bool chromeStillRunning = false;
                bool firefoxStillRunning = false;

                Process[] processlist = Process.GetProcesses();

                Console.WriteLine("Start " + DateTime.Now.ToShortTimeString());
                foreach (Process processItem in processlist)
                {
                    switch (processItem.ProcessName)
                    {
                        case "SketchUp":
                            toDo.AddOrUpdate(processItem as ProcessToRecord, true);

                            sketchupStartTime = processItem.StartTime;
                            sketchupStillRunning = true;
                            Program.LogDetect(processItem);
                            break;

                        case "devenv":
                            //toDo.AddOrUpdate(processItem.Id, true);

                            devEnvStartTime = processItem.StartTime;
                            devEnvStillRunning = true;
                            Program.LogDetect(processItem);
                            break;

                        case "chrome":
                            //toDo.AddOrUpdate(processItem.Id, true);

                            chromeStartTime = processItem.StartTime;
                            chromeStillRunning = true;
                            Program.LogDetect(processItem);
                            break;

                        case "firefox":
                            //toDo.AddOrUpdate(processItem.Id, true);

                            firefoxStartTime = processItem.StartTime;
                            firefoxStillRunning = true;
                            Program.LogDetect(processItem);
                            break;

                        default:
                            break;
                    }
                }

                //ToDo.Log() //goes through internal list and logs everything in it.

                if ((!sketchupStillRunning) && (sketchupStartTime != new DateTime()))
                {
                    Console.WriteLine("Sketchup presumed stopped. Logging to Calendar");
                    Program.CreateEntry(userName, password, "Google Sketchup", "", sketchupStartTime, DateTime.Now);
                    Console.WriteLine("Sketchup activity logged " + DateTime.Now.ToShortTimeString());
                    sketchupStartTime = new DateTime();
                }

                if ((!devEnvStillRunning) && (devEnvStartTime != new DateTime()))
                {
                    Console.WriteLine("Visual Studio presumed stopped. Logging to Calendar");
                    Program.CreateEntry(userName, password, "Visual Studio", "", devEnvStartTime, DateTime.Now);
                    Console.WriteLine("Visual Studio activity logged " + DateTime.Now.ToShortTimeString());
                    devEnvStartTime = new DateTime();
                }

                if ((!chromeStillRunning) && (chromeStartTime != new DateTime()))
                {
                    Console.WriteLine("Chrome presumed stopped. Logging to Calendar");
                    Program.CreateEntry(userName, password, "Google Chrome", "", chromeStartTime, DateTime.Now);
                    Console.WriteLine("Chrome activity logged " + DateTime.Now.ToShortTimeString());
                    chromeStartTime = new DateTime();
                }

                if ((!firefoxStillRunning) && (firefoxStartTime != new DateTime()))
                {
                    Console.WriteLine("Firefox presumed stopped. Logging to Calendar");
                    Program.CreateEntry(userName, password, "Mozilla Firefox", "", firefoxStartTime, DateTime.Now);
                    Console.WriteLine("Firefox activity logged " + DateTime.Now.ToShortTimeString());
                    firefoxStartTime = new DateTime();
                }

                Console.WriteLine("Sleeping for 1 minute");
                Thread.Sleep(new TimeSpan(0,0,1,0));   
            }
        }

        private static void LogDetect(Process processItem)
        {
            Console.WriteLine(processItem.MainWindowTitle + DETECTED_RUNNING + DateTime.Now.ToShortTimeString());
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
                AtomEntry insertedEntry = _service.Insert(postUri, entry);

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