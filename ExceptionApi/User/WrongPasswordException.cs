namespace ApiCos.ExceptionApi.User
{
    public class WrongPasswordException : BaseException
    {
        private const int ErrorId = 103;
        private const string ErrorMessage = "Wrong password";
        public WrongPasswordException() : base(ErrorId, ErrorMessage)
        {
        }
    }
}
