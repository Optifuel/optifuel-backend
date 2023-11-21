namespace ApiCos.ExceptionApi.User
{
    public class WrongValidationTokenException : BaseException
    {
        private const int ErrorId = 107;
        private const string ErrorDescription = "Wrong Token";

        public WrongValidationTokenException() : base(ErrorId, ErrorDescription)
        {
        }
    }
}
