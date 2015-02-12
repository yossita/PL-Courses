using System.Threading;

namespace PL_Course.Integration.Workflows
{
    public class UnsubscribeCrmWorkflow
    {

        public string Email { get; private set; }

        public UnsubscribeCrmWorkflow(string email)
        {
            Email = email;
        }


        public void Run()
        {
            Thread.Sleep(UnsubscribeWorkflow.StepDuration);
        }
    }
}