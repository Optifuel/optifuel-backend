namespace Api.ExceptionApi
{
    [Serializable]
    public class BaseException : Exception
    {
        public int id;
        public string description;
        public BaseException(int id, string description) : base(description)
        {
            this.id = id;
            this.description = description;
        }
    }
}
