using Experimental.System.Messaging;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TilmeldAktivitet
{
    public class Medlemsregister
    {
        public MessageQueue _requestQueue;
        public MessageQueue _replyQueue;

        public Medlemsregister(MessageQueue requestQueue, MessageQueue replyQueue)
        {
            _requestQueue = requestQueue;
            _replyQueue = replyQueue;
        }

        public void Start()
        {
            Console.WriteLine("Medlemsregister lytter på request køen...");

            while (true)
            {
                _requestQueue.Formatter = new XmlMessageFormatter(new Type[] { typeof(string) });
                     
                Message request = _requestQueue.Receive();
                request.Formatter = new XmlMessageFormatter( new Type[] {typeof(string)});

                string requestBody = (string)request.Body;
                Console.WriteLine(requestBody);
                //string medlemsnummer = (string)requestBody.Medlemsnummer();

                Console.WriteLine("Modtaget besked fra request!");

                //bool erMedlem = medlemsnummer == "1221";

                bool erMedlem = true;
                string medlemsNummer = "1221";

                var messageBody = new { Medlemsnummer = medlemsNummer, ErMedlem = erMedlem };

                string jsonBody = JsonConvert.SerializeObject(messageBody);


                Message message = new Message()
                {
                    Body = jsonBody,
                    Label = "Response medlemsskab"
                };

                _replyQueue.Send(message);
                Console.WriteLine("Sendt response message");

            }
        }
    }
}
