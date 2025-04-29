namespace BTL_Web.Models
{
    public class MessageOverviewViewModel
    {
        public User User { get; set; }
        public Message LastMessage { get; set; }
        public string Direction { get; set; } // thêm chiều tin nhắn
    }

}
