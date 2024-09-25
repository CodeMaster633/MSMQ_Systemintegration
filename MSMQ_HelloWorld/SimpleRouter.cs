using Experimental.System.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MSMQ_HelloWorld
{

    class SimpleRouter
    {
        protected MessageQueue inQueue;
        protected MessageQueue outQueue1;
        protected MessageQueue outQueue2;
        protected MessageQueue outQueue3;

        public SimpleRouter(MessageQueue inQueue, MessageQueue outQueue1, MessageQueue outQueue2, MessageQueue outQueue3)
        {
            this.inQueue = inQueue;
            this.outQueue1 = outQueue1;
            this.outQueue2 = outQueue2;
            this.outQueue3 = outQueue3;

            inQueue.ReceiveCompleted += new ReceiveCompletedEventHandler(OnMessage);
            inQueue.BeginReceive();
        }
        private void OnMessage(Object source, ReceiveCompletedEventArgs asyncResult)
        {
            MessageQueue mq = (MessageQueue)source;
            Message message = mq.EndReceive(asyncResult.AsyncResult);
            try
            {
                if (WhichCompany(message) == 1)
                {
                    outQueue1.Send(message);
                    Console.WriteLine("SAS kø modtaget besked");
                }
                else if (WhichCompany(message) == 2)
                {
                    outQueue2.Send(message);
                    Console.WriteLine("KLM kø modtaget besked");
                }
                else if (WhichCompany(message) == 3)
                {
                    outQueue2.Send(message);
                    Console.WriteLine("SW kø modtaget besked");
                }
                else
                {
                    Console.WriteLine("Der opstod en fejl");
                }
                mq.BeginReceive();
            }
            catch (Exception e)
            {
                Console.WriteLine("wuups " + e.InnerException);
            }
        }
        protected int WhichCompany(Message message)
        {
            string destination = message.Label;

            if (destination == "SAS") return 1;
            else if (destination == "KLM") return 2;
            else if (destination == "SW") return 2;
            else return 0;
        }
    }
}
    