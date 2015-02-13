using System.Threading;
using PL_Course.Messages.Events;
using PL_Course.Messaging;
using PL_Course.Messaging.Spec;

namespace PL_Course.Integration.Workflows
{
    public class UnsubscribeWorkflow
    {
        public const int StepDuration = 2000;

        public string Email { get; private set; }

        public UnsubscribeWorkflow(string email)
        {
            Email = email;
        }


        public void Run()
        {
            SendNotificationEvent(Email);
            CancelPendingMailShots();
        }

        private void SendNotificationEvent(string email)
        {
            var evt = new UserUnsubscribed { EmailAddress = email };
            var queue = MessageQueueFactory.CreateOutbound("unsubscribed-event", MessagePattern.PublishSubscribe);
            queue.Send(new Message() { Body = evt });
        }

        private void CancelPendingMailShots()
        {
            Thread.Sleep(StepDuration);
        }



    }
}