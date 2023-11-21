namespace ApiCos.ExceptionApi.Company
{
    public class VatNumberNotValid : BaseException
    {
        private const int ErrorId = 201;
        private const string ErrorDescription = "Vat Number Not Valid";

        public VatNumberNotValid() : base(ErrorId, ErrorDescription)
        {
        }
    }
}
