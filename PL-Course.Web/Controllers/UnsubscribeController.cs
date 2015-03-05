using System.Threading;
using System.Web.Mvc;
using PL_Course.Messages.Commands;
using PL_Course.Messages.Queries;
using PL_Course.Messaging;
using PL_Course.Messaging.Spec;

namespace PL_Course.Web.Controllers
{
    public class UnsubscribeController : Controller
    {

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Submit(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                ModelState.AddModelError("email", "Email can't be empty");
                return View("Index");
            }

            if (DoesUserExist(email))
            {
                StartUnsubscribe(email);
                return View("Confirmation");
            }
            return View("Unknown");
        }

        private bool DoesUserExist(string email)
        {
            var exists = false;

            var queue = MessageQueueFactory.CreateOutbound("doesuserexist", MessagePattern.RequestResponse);
            var responseQueue = queue.GetResponseQueue();

            queue.Send(new Message()
            {
                Body = new DoesUserExistRequest() { Email = email },
                ResponseAddress = responseQueue.Address
            });
            responseQueue.Receive(m => exists = m.BodyAs<DoesUserExistResponse>().Exists);

            return exists;
        }

        private static void StartUnsubscribe(string email)
        {
            var queue = MessageQueueFactory.CreateOutbound("unsubscribe", MessagePattern.FireAndForget);

            queue.Send(new Message()
            {
                Body = new UnsubscribeCommand() { Email = email },
                ResponseAddress = queue.GetResponseQueue().Address
            });
        }

        public ActionResult SubmitSync(string email)
        {
            var workflow = new UnsubscribeWorkflow(email);
            workflow.Run();
            return View("Confirmation");
        }
    }

    public class UnsubscribeWorkflow
    {
        private const int StepDuration = 10000;

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