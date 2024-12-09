namespace Api.ExceptionApi.Vehicle
{
    public class LicensePlateEmptyException : BaseException
    {
        private const int ErrorId = 302;
        private const string ErrorDescription = "LicensePlate is empty";

        public LicensePlateEmptyException():base(ErrorId, ErrorDescription) { }
    }
}
