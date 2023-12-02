namespace ApiCos.ExceptionApi.MapBox
{
    public class DeserializarMapBoxException : BaseException
    {
        private const int ErrorId = 501;
        private const string ErrorDescription = "Error while deserializing the response from MapBox";
        public DeserializarMapBoxException() : base(ErrorId, ErrorDescription)
        {
        }

    }
}
