using System.Threading;
using System.Web.Mvc;

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