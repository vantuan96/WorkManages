namespace Kztek_Core.Models
{
    public class MessageReport
    {
        public bool isSuccess { get; set; }

        public string Message { get; set; }

        public MessageReport()
        {

        }

        public MessageReport(bool isSuccess, string Message)
        {
            this.isSuccess = isSuccess;
            this.Message = Message;
        }
    }
}