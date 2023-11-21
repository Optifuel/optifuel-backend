using ApiCos.ExceptionApi;

namespace ApiCos.ExceptionApi.User
{
    public class UserNotFoundException : BaseException
    {
        private const int ErrorId = 101;
        private const string ErrorDescription = "User not found";

        public UserNotFoundException() : base(ErrorId, ErrorDescription)
        {
        }

    }
}
