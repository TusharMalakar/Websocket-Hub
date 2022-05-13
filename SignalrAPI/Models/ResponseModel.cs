namespace SignalrAPI.Models
{
    public class ResponseModel
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public object Data { get; set; }
        public ResponseModel()
        {
            Success = false;
            Message = "Firm currently offline";
        }
    }
}
