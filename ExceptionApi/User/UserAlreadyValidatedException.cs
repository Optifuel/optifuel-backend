namespace Api.ExceptionApi.User
{
    public class UserAlreadyValidatedException : BaseException
    {
        private const int ErrorId = 106;
        private const string ErrorDescription = "User already validated";

        public UserAlreadyValidatedException() : base(ErrorId, ErrorDescription)
        {
        }
    }
}
