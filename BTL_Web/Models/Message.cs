namespace BTL_Web.Models
{
    public class Message
    {
        public int Id { get; set; }

        public int SenderId { get; set; }  // Id người gửi (user hoặc consultant)
        public string SenderRole { get; set; }  // "User" hoặc "Consultant"

        public int ReceiverId { get; set; }
        public string ReceiverRole { get; set; }  // "User" hoặc "Consultant"

        public string Content { get; set; }
        public DateTime Timestamp { get; set; }
    }

}
