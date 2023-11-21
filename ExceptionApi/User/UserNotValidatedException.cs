using ApiCos.ExceptionApi;

namespace ApiCos.ExceptionApi.User
{
    public class UserNotValidatedException : BaseException
    {
        private const int ErrorId = 102;
        private const string ErrorDescription = "Account is not validated";

        public UserNotValidatedException() : base(ErrorId, ErrorDescription)
        {
        }
    }
}
