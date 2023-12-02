namespace ApiCos.ExceptionApi.MapBox
{
    public class NoGasStationFoundException : BaseException
    {
        private const int ErrorId = 502;
        private const string ErrorDescription = "No Gas Station Found";
        public NoGasStationFoundException() : base(ErrorId, ErrorDescription)
        {
        }
    }
}
