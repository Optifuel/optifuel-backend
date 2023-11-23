namespace ApiCos.ExceptionApi.User
{
    public class WrongChangePasswordTokenException : BaseException
    {
        private const int ErrorId = 110;
        private const string ErrorDescription = "Wrong Token";

        public WrongChangePasswordTokenException() : base(ErrorId, ErrorDescription) { }
    }
}
