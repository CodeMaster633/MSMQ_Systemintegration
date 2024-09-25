using Experimental.System.Messaging;
using L08_Request_Reply;

class Program
{

    static void Main()
    {

        SAS sas = new SAS();
        AIC aic = new AIC();

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

            while (true)
            {
                Thread aicThread = new Thread(() => aic.AICReply(requestQueue, replyQueue));
                aicThread.Start();
            }
        }

    }
}