namespace Api.ExceptionApi.MapBox
{
    public class NotEnoughPointsException : BaseException
    {
        private const int ErrorId = 532;
        private const string ErrorDescription = "Not Enough Points";
        public NotEnoughPointsException() : base(ErrorId, ErrorDescription)
        {
        }
    }
}