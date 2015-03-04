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
            var queue = MessageQueueFactory.CreateInbound("unsubscribe-crm", MessagePattern.PublishSubscribe);
            Console.WriteLine("Listening on {0}", queue.Address);
            queue.Listen(message =>
            {
                var evt = message.BodyAs<UserUnsubscribed>();
                Console.WriteLine("Received UserUnsubscribed event for: {0}, at {1}", evt.EmailAddress, DateTime.Now);
                new UnsubscribeCrmWorkflow(evt.EmailAddress).Run();
                Console.WriteLine("Processed UserUnsubscribed event for: {0}, at {1}", evt.EmailAddress, DateTime.Now);
            });
        }
    }
}
