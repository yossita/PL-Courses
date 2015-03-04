using System;
using PL_Course.Integration.Workflows;
using PL_Course.Messages.Events;
using PL_Course.Messaging;
using PL_Course.Messaging.Spec;

namespace PL_Course.Handlers.UnsubscribeCrm
{
    class Program
    {
        static void Main(string[] args)
        {
            var queueAddress = ".\\private$\\unsubscribe-crm";
            var multicastAddress = "234.1.1.2:8001";
            using (var queue = new MessageQueue(queueAddress))
            {
                var evt = message.BodyAs<UserUnsubscribed>();
                        Console.WriteLine("Received UserUnsubscribed event for: {0}, at {1}", evt.EmailAddress, DateTime.Now);
                        new UnsubscribeCrmWorkflow(evt.EmailAddress).Run();
                        Console.WriteLine("Processed UserUnsubscribed event for: {0}, at {1}", evt.EmailAddress, DateTime.Now);
            });
        }
    }
}
