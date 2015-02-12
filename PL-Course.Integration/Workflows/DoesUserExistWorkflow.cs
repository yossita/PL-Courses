namespace PL_Course.Integration.Workflows
{
    public class DoesUserExistWorkflow
    {

        public bool DoesUserExists(string email)
        {
            return email.EndsWith("gmail.com");
        }
    }
}