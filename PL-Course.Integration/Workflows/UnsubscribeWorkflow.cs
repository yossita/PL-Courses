using System.Messaging;
using System.Threading;
using PL_Course.Infrastructure;
using PL_Course.Messages.Events;

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
            PersistAsUnsubscribed();
            UnsubscribeInLegacySystem();
            SetCrmMailingPreference();
            CancelPendingMailShots();
        }

        private void NotifyUserUnsibscribed()
        {
            var evt = new UserUnsubscribed { EmailAddress = Email };
            using (var queue = new MessageQueue("FormateName:MULTICAST=234.1.1.2:8001"))
            {
                var message = new Message();
                message.BodyStream = evt.ToJsonStream();
                message.Label = evt.GetMessageType();
                message.Recoverable = true;
                queue.Send(message);
            }
        }

        private void CancelPendingMailShots()
        {
            Thread.Sleep(StepDuration);
        }

        private void SetCrmMailingPreference()
        {
            Thread.Sleep(StepDuration);
        }

        private void UnsubscribeInLegacySystem()
        {
            Thread.Sleep(StepDuration);
        }

        private void PersistAsUnsubscribed()
        {
            Thread.Sleep(StepDuration);
        }

    }
}