namespace WebAPI.Errors
{
    public class CodeErrorException : CodeErrorResponse
    {
        public string details { get; set; }



        public CodeErrorException(int statusCode, string message = null, string details = null) : base(statusCode, message)
        {
            this.details = details;
        }

    }
}
