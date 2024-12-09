namespace Api.ExceptionApi.Vehicle
{
    public class VehicleNotFoundException : BaseException
    {
        private const int ErrorId = 301;
        private const string ErrorDescription = "Vehicle Not Found";

        public VehicleNotFoundException() : base(ErrorId, ErrorDescription)
        {

        }
    }
}
