namespace ApiCos.ExceptionApi.Company
{
    public class CompanyNotFoundException : BaseException
    {
        private const int ErrorId = 202;
        private const string ErrorDescription = "Company Not Found";

        public CompanyNotFoundException() : base(ErrorId, ErrorDescription)
        {
        }
    }
}
