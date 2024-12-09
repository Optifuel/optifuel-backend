namespace Api.ExceptionApi.Company
{
    public class VatNumberEmptyException : BaseException
    {
        private const int ErrorId = 203;
        private const string ErrorDescription = "Vat Number is Empty";

        public VatNumberEmptyException() : base(ErrorId, ErrorDescription)
        {
        }
    }
}
