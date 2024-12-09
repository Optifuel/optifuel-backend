namespace Api.ExceptionApi.Company
{
    public class BussinessnameEmptyException : BaseException
    {
        private const int ErrorId = 204;
        private const string ErrorDescription = "Bussinessname Empty";

        public BussinessnameEmptyException() : base(ErrorId, ErrorDescription)
        {
        }
    }
}
