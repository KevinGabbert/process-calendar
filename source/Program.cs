using System;
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
        static void Main(string[] args)
        {
            Console.WriteLine("Process Calendar v9.22.10 (proof of concept)" + DateTime.Now.ToShortTimeString());

            const string userName = "uuuuu";
            const string password = "uuuuu";

            DateTime sketchupStartTime = new DateTime();
            DateTime devEnvStartTime = new DateTime();
            DateTime chromeStartTime = new DateTime();

            while (true)
            {
                bool sketchupStillRunning = false;
                bool devEnvStillRunning = false;
                bool chromeStillRunning = false;

                Process[] processlist = Process.GetProcesses();

                Console.WriteLine("Start " + DateTime.Now.ToShortTimeString());
                foreach (Process processItem in processlist)
                {
                    //Console.WriteLine("{0}", processItem.ProcessName);

                    switch (processItem.ProcessName)
                    {
                        case "SketchUp":
                            sketchupStartTime = processItem.StartTime;
                            sketchupStillRunning = true;
                            Console.WriteLine("Sketchup detected running " + DateTime.Now.ToShortTimeString());
                            break;

                        case "devenv":
                            devEnvStartTime = processItem.StartTime;
                            devEnvStillRunning = true;
                            Console.WriteLine("Visual Studio detected running " + DateTime.Now.ToShortTimeString());
                            break;

                        case "chrome":
                            chromeStartTime = processItem.StartTime;
                            chromeStillRunning = true;
                            Console.WriteLine("Chrome detected running " + DateTime.Now.ToShortTimeString());
                            break;

                        default:
                            break;
                    }
                }


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
                    Console.WriteLine("Chrome presume stopped. Logging to Calendar");
                    Program.CreateEntry(userName, password, "Google Chrome", "", chromeStartTime, DateTime.Now);
                    Console.WriteLine("Chrome activity logged " + DateTime.Now.ToShortTimeString());
                    chromeStartTime = new DateTime();
                }

                Console.WriteLine("Sleeping for 1 minute");
                Thread.Sleep(new TimeSpan(0,0,1,0));   
            }
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

                Uri postUri = new Uri("http://www.google.com/calendar/feeds/kevingabbert@gmail.com/private/full");

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
