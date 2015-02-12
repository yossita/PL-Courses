using System.Threading;

namespace PL_Course.Integration.Workflows
{
    public class UnsubscribeFulfillmentWorkflow
    {

        public string Email { get; private set; }

        public UnsubscribeFulfillmentWorkflow(string email)
        {
            Email = email;
        }


        public void Run()
        {
            Thread.Sleep(UnsubscribeWorkflow.StepDuration);
        }
    }
}