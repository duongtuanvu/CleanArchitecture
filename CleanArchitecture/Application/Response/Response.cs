namespace Application.Response
{
    public class Response
    {
        public Response()
        {

        }
        public Response(string message = null, object data = null, object errors = null)
        {
            Message = message;
            Data = data;
            Errors = errors;
        }
        public string Message { get; set; }
        public object Data { get; set; }
        public object Errors { get; set; }
    }
}
