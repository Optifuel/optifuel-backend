namespace Api.ExceptionApi.User
{
    public class PasswordEmptyException : BaseException
    {
        private const int ErrorId = 105;
        private const string ErrorDescription = "Password is empty";
        public PasswordEmptyException() : base(ErrorId, ErrorDescription)
        {
        }
    }
}
