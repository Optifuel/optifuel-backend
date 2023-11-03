namespace ApiCos.Response
{
    public class ApiResponse
    {
        public string Code { get; set; }
        public string Message { get; set; } = null!;
        public object Data { get; set; } = null!;
    }

    public enum ResponseType
    {
        Success,
        NotFound,
        Failure
    }
}
