namespace WebAPI.Errors
{
    public class CodeErrorResponse
    {

        public int statusCode { get; set; }
        public string message { get; set; }

        public CodeErrorResponse(int statusCode, string message = null)
        {
            this.statusCode = statusCode;
            this.message = message ?? GetDefaultMessageStatus(statusCode);
        }

        private string GetDefaultMessageStatus(int statusCode)
        {
            return statusCode switch
            {
                400 => "Bad request",
                401 => "Not Authorized",
                404 => "Resource not found",
                500 => "Error server",
                _ => null
            };
        }

    }
}
