namespace BTL_Web.Models
{
    public class ChatSummaryViewModel
    {
        public int UserId { get; set; }
        public string Username { get; set; }

        public int ConsultantId { get; set; }
        public string ConsultantName { get; set; }

        public DateTime LastMessageTime { get; set; }

        public string LastMessageContent { get; set; }
    }





}
