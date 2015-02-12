using System;
using System.Messaging;
using PL_Course.Infrastructure;
using PL_Course.Integration.Workflows;
using PL_Course.Messages.Events;

namespace PL_Course.Handlers.UnsubscribeLegacy
{
    class Program
    {
        static void Main(string[] args)
        {
            var queueAddress = ".\\private$\\unsubscribe-legacy";
            var multicastAddress = "234.1.1.2:8001";
            using (var queue = new MessageQueue(queueAddress))
            {
                queue.MulticastAddress = multicastAddress;
                while (true)
                {
                    Console.WriteLine("Listening on {0}", queueAddress);
                    var message = queue.Receive();
                    var messageBody = message.BodyStream.ReadFromJsonStream(message.Label);
                    if (messageBody.GetType() == typeof(UserUnsubscribed))
                    {
                        var evt = ((UserUnsubscribed)messageBody);
                        Console.WriteLine("Received UserUnsubscribed event for: {0}, at {1}", evt.EmailAddress, DateTime.Now);
                        new UnsubscribeLegacyWorkflow(evt.EmailAddress).Run();
                        Console.WriteLine("Processed UserUnsubscribed event for: {0}, at {1}", evt.EmailAddress, DateTime.Now);
                    }
                }
            }
        }
    }
}
