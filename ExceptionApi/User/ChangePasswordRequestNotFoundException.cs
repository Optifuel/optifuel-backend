namespace Api.ExceptionApi.User
{
    public class ChangePasswordRequestNotFoundException : BaseException
    {
        private const int ErrorId = 109;
        private const string ErrorDescription = "Change Password Request Not Found";
        public ChangePasswordRequestNotFoundException() : base(ErrorId, ErrorDescription)
        {
        }
    }
}
