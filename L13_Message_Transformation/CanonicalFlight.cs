using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace L13_Message_Transformation
{
    public class CanonicalFlight
    {
        public string Airline { get; set; }
        public string FlightNumber { get; set; }
        public string Destination { get; set; }
        public string Origin { get; set; }
        public string ArrivalDeparture { get; set; }
        public DateTime Date { get; set; }
        public TimeSpan Time { get; set; }
    }
}
