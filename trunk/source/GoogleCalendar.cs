using System;
using Google.GData.Calendar;
using Google.GData.Client;
using Google.GData.Extensions;

namespace ProcessCalendar
{
    class GoogleCalendar
    {
        public static Uri _calendarToPost = new Uri("http://www.google.com/calendar/feeds/default/private/full");
        private static readonly Google.GData.Calendar.CalendarService _service = new CalendarService("processLogService");

        public static CalendarFeed RetrieveCalendars(string userName, string password)
        {
            // Create a CalenderService and authenticate
            var myService = new CalendarService("process-calendar");
            myService.setUserCredentials(userName, password);

            var query = new CalendarQuery();
            query.Uri = new Uri("http://www.google.com/calendar/feeds/default/owncalendars/full");

            return myService.Query(query);
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
    }
}
