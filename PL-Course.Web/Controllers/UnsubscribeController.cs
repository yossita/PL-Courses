using System;
using System.Messaging;
using System.Threading;
using System.Web.Mvc;
using PL_Course.Infrastructure;
using PL_Course.Messages.Commands;
using PL_Course.Messages.Queries;

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
            var responseAddress = ".\\private$\\" + Guid.NewGuid().ToString().Substring(0, 6);
            try
            {
                using (var responseQueue = MessageQueue.Create(responseAddress))
                {
                    var request = new DoesUserExistRequest { Email = email };
                    using (var queue = new MessageQueue(".\\private$\\doesuserexist"))
                    {
                        var message = new Message();
                        message.BodyStream = request.ToJsonStream();
                        message.Label = request.GetMessageType();
                        message.ResponseQueue = responseQueue;
                        queue.Send(message);
                    }
                    var response = responseQueue.Receive();
                    return response.BodyStream.ReadFromJsonStream<DoesUserExistResponse>().Exists;
                }
            }
            finally
            {
                if (MessageQueue.Exists(responseAddress)) MessageQueue.Delete(responseAddress);
            }
        }

        private static void StartUnsubscribe(string email)
        {
            var command = new UnsubscribeCommand() { Email = email };

            using (var queue = new MessageQueue(".\\private$\\unsubscribe"))
            {
                var message = new Message();
                message.BodyStream = command.ToJsonStream();
                message.Label = command.GetMessageType();
                //using (var tx = new MessageQueueTransaction())
                {
                    //tx.Begin();
                    //queue.Send(message, tx);
                    //tx.Commit();
                    queue.Send(message);
                }
            }
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