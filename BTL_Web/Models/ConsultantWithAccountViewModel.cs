namespace BTL_Web.Models
{
    public class ConsultantWithAccountViewModel
    {
        public int Id { get; set; }
        public string ImageUrl { get; set; }
        public string Name { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public string Expertise { get; set; }

        // Thông tin tài khoản
        public string Username { get; set; }
        public string Password { get; set; }
    }

}
