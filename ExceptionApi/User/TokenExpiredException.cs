namespace ApiCos.ExceptionApi.User
{
    public class TokenExpiredException : BaseException
    {
        private const int ErrorId = 108;
        private const string ErrorDescription = "Token Expired";
        public TokenExpiredException() : base(ErrorId, ErrorDescription)
        {
        }
    }
}
