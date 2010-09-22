using System;
using Google.GData.Calendar;
using Google.GData.Client;
using Google.GData.Extensions;
using System.Net;

namespace ProcessCalendar
{
    class Program
    {
        static void Main(string[] args)
        {
            Program.CreateEntry("xxxxx", "xxxxxx", "TEST automated Entry", "test automated description", DateTime.Now, DateTime.Now.AddHours(1));

            //Process[] processlist = Process.GetProcesses();

            //foreach(Process processItem in processlist)
            //{
            //    Console.WriteLine("Process: {0} ID: {1}", processItem.ProcessName, processItem.Id);

            //    //Set start flag

            //    switch (processItem.ProcessName)
            //    {
            //        case "SketchUp":
            //            //grab processItem.StartTime
            //            //if flag is not set, then set a flag.  (SketchUp)
            //            //else check to see if it has the same start time.
            //            //yes? ignore.
            //            //no? set CREATE ENTRY flag, set flag to true
            //            break;

            //        case "devenv":
            //            //grab processItem.StartTime
            //            //set a flag.  (devenv)
            //            //else check to see if it has the same start time.
            //            //yes? ignore.
            //            //no? set CREATE ENTRY flag, set flag to true
            //            break;

            //        case "chrome":
            //            //grab processItem.StartTime
            //            //set a flag. (chrome)
            //            //else check to see if it has the same start time.
            //            //yes? ignore.
            //            //no? set CREATE ENTRY flag, set flag to true
            //            break;

            //        default:
            //            break;
            //    }

            //    //set END flag
                
            //    //Do we have any CREATE ENTRY flags?  Great, create entry

            //}

            //Console.ReadKey(true);

            //p.StartTime (Shows the time the process started)
            //p.TotalProcessorTime (Shows the amount of CPU time the process has taken)
            //p.Threads ( gives access to the collection of threads in the process)


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
