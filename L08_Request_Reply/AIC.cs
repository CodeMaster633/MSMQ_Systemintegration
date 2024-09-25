using Experimental.System.Messaging;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace L08_Request_Reply
{
    class AIC
    {
        public AIC() { }
        public void AICReply(MessageQueue queue, MessageQueue queue2)
        {

            try
            {
                queue.Formatter = new XmlMessageFormatter(new Type[] { typeof(string) });

                Message modtagetMessage = queue.Receive();
                Console.WriteLine("Request besked modtaget: " + (string)modtagetMessage.Body );
                Console.WriteLine(" Correlation ID : " +modtagetMessage.Id);

                SendReply(queue2);
            }
            catch (Exception ex)
            {
                Console.WriteLine("fejl i AIC"+ex.Message);
            }
        }

        public static void SendReply(MessageQueue queue)
        {
            try
            {

                // Opret beskeden
                Message message = new Message();
                var messageBody = new
                {
                    flightno = "SK345",
                    ETA = "17:50"
                };
                string jsonBody = JsonConvert.SerializeObject(messageBody);

                message.Body = jsonBody;
                message.Label = "Reply af ETA";

                // Send beskeden til køen
                queue.Send(message);

                Console.WriteLine("Besked sendt til reply kø!");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Fejl Sendreply" + ex.Message);
            }
        }
    }
}