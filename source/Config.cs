using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace ProcessCalendar
{
    class Config
    {
        public static IEnumerable<XElement> GetWatchList(string filePath)
        {
            return from p in XElement.Load(filePath).Elements("Process")
                   select p;
        }
    }
}
