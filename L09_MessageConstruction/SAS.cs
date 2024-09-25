using Experimental.System.Messaging;
using Newtonsoft.Json;
namespace L09_MessageConstruction
{
    class SAS
    {

        public SAS() { }

        public void SASRequest(MessageQueue requestQueue, MessageQueue replyQueue)
        {

            // Opret beskeden
            Message message = new Message();
            var messageBody = new
            {
                flightno = "SK345",
            };
            string jsonBody = JsonConvert.SerializeObject(messageBody);

            message.Body = jsonBody;
            message.Label = "Request af ETA";
            //message.TimeToBeReceived = TimeSpan.FromSeconds(10);

            // Send beskeden til køen
            requestQueue.Send(message);

            Console.WriteLine("Besked sendt!");

            string requestId = message.Id;

            //Opsæt modtagelse q2
            replyQueue.Formatter = new XmlMessageFormatter(new String[] { "System.String" });
            replyQueue.ReceiveCompleted += (sender, e) => OnMessage(sender, e, requestId);
            replyQueue.BeginReceive();

        }
        public void OnMessage(object source, ReceiveCompletedEventArgs e, string requestId)
        {
            try
            {
                MessageQueue queue = (MessageQueue)source;
                Message message = queue.EndReceive(e.AsyncResult);

                if (message.CorrelationId == requestId)
                {
                    Console.WriteLine("Svar besked modtaget med korrekt CorrelationId: " + (string)message.Body);
                }
                else
                {
                    Console.WriteLine("Modtaget svar besked med forkert CorrelationId");
                }

                queue.Formatter = new XmlMessageFormatter(new Type[] { typeof(string) });

                Console.WriteLine("Reply besked modtaget: " + (string)message.Body);

                queue.BeginReceive();

            }
            catch (Exception ex)
            {
                Console.WriteLine("Fejl... " + ex.Message);
            }
        }
    }
}
