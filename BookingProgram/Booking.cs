using Experimental.System.Messaging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TilmeldAktivitet
{
    public class Booking
    {
        public MessageQueue _requestQueue;
        public MessageQueue _replyQueue;


        public Booking(MessageQueue requestQueue, MessageQueue replyQueue)
        {
            _requestQueue = requestQueue;
            _replyQueue = replyQueue;
        }

        public void Start()
        {
            var messageBody = new { Medlemsnummer = "1221" };

            string jsonBody = JsonConvert.SerializeObject(messageBody);


            Message message = new Message() {
                Body = jsonBody,
                Label = "Request medlemsskab",
                ResponseQueue = _requestQueue
            };

            _requestQueue.Send(message);
            Console.WriteLine("Booking sendt besked ");

            var response = _replyQueue.Receive(TimeSpan.FromSeconds(4));
            response.Formatter = new XmlMessageFormatter(new Type[] { typeof(string) });
            dynamic responseBody = response.Body;

            Console.WriteLine("Booking modtaget besked. Respons: " + responseBody);
        }
    }
}
