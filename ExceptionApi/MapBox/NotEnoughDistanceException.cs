namespace Api.ExceptionApi.MapBox
{
    public class NotEnoughDistanceException : BaseException
    {
        private const int ErrorId = 533;
        private const string ErrorDescription = "Not Necessary Find Gas Station";
        public NotEnoughDistanceException() : base(ErrorId, ErrorDescription)
        {
        }
    }
}