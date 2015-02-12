using System.Threading;

namespace PL_Course.Integration.Workflows
{
    public class UnsubscribeLegacyWorkflow
    {

        public string Email { get; private set; }

        public UnsubscribeLegacyWorkflow(string email)
        {
            Email = email;
        }


        public void Run()
        {
            Thread.Sleep(UnsubscribeWorkflow.StepDuration);
        }
    }
}