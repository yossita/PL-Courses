namespace PL_Course.Messages.Queries
{
    public class DoesUserExistRequest
    {
        public string Email { get; set; }
    }

    public class DoesUserExistResponse
    {
        public bool Exists { get; set; }
    }
}
