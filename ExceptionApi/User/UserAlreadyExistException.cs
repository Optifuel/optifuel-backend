namespace Api.ExceptionApi.User
{
    public class UserAlreadyExistException : BaseException
    {
        private const int ErrorId = 120;
        private const string ErrorDescription = "User Already Exist Exception";
        public UserAlreadyExistException() : base(ErrorId, ErrorDescription)
        {
        }
    }
}
