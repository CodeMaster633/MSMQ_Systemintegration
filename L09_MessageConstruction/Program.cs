using Experimental.System.Messaging;

using L09_MessageConstruction;

class Program
{

    static void Main()
    {

        SAS sas = new SAS();

        string queuePath = @".\Private$\RequestKø";
        string queuePath2 = @".\Private$\ReplyKø";

        // Opret køen, hvis den ikke allerede findes
        if (!MessageQueue.Exists(queuePath))
        {
            MessageQueue.Create(queuePath);
        }
        if (!MessageQueue.Exists(queuePath2))
        {
            MessageQueue.Create(queuePath2);
        }

        // Opret en forbindelse til køen
        using (MessageQueue requestQueue = new MessageQueue(queuePath))
        using (MessageQueue replyQueue = new MessageQueue(queuePath2))

        {
            Thread sasThread = new Thread(() => sas.SASRequest(requestQueue, replyQueue));
            sasThread.Start();

        }

    }
}