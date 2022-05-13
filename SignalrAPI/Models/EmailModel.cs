namespace SignalrAPI.Models
{
    public class EmailModel
    {
        //[Required]
        public string To { get; set; }
        //[Required]
        public string From { get; set; }
        public object Data { get; set; }
    }
}
