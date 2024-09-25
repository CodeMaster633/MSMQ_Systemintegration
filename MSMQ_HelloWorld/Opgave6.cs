using Experimental.System.Messaging;
using Newtonsoft.Json;

namespace MSMQ_HelloWorld;
class Opgave6
{
    static void Main()
    {
        string queuePath = @".\Private$\AirportInformationCenter";
        string queuePathOut1 = @".\Private$\SAS";
        string queuePathOut2 = @".\Private$\KLM";
        string queuePathOut3 = @".\Private$\SW";

        // Opret køen, hvis den ikke allerede findes
        if (!MessageQueue.Exists(queuePath)) MessageQueue.Create(queuePath);
        if (!MessageQueue.Exists(queuePathOut1)) MessageQueue.Create(queuePathOut1);
        if (!MessageQueue.Exists(queuePathOut2)) MessageQueue.Create(queuePathOut2);
        if (!MessageQueue.Exists(queuePathOut3)) MessageQueue.Create(queuePathOut3);


        // Opret en forbindelse til køen
        using (MessageQueue queue = new MessageQueue(queuePath))
        {
            using (MessageQueue queueOut1 = new MessageQueue(queuePathOut1))
            using (MessageQueue queueOut2 = new MessageQueue(queuePathOut2))
            using (MessageQueue queueOut3 = new MessageQueue(queuePathOut3))

            {
                // Opret beskeden
                var messageBody = new
                {
                    planlagt_tid = "17:20/20:10",
                    flightno = "SK345",
                    destination = "Amsterdam",
                    checkIn = "Open",
                    gateNo = "C14"
                };

                string jsonBody = JsonConvert.SerializeObject(messageBody);

                Message message = new Message();
                message.Body = jsonBody;
                message.Label = "KLM";

                // Send beskeden til køen
                queue.Send(message, message.Label);
                Console.WriteLine("Besked sendt!");

                SimpleRouter router = new SimpleRouter(queue, queueOut1, queueOut2, queueOut3);

                // Hold programmet i gang, så routeren kan køre
                Console.WriteLine("Tryk på en tast for at afslutte...");
                Console.ReadKey();

            }
        }
    }
}


