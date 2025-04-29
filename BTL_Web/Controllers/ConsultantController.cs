using Microsoft.AspNetCore.Mvc;
using BTL_Web.Models;
using System.Linq;
using BTL_Web.Data;
// đây là trang tư vấn của người dùng
namespace BTL_Web.Controllers
{
    public class ConsultantController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ConsultantController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Trang hiển thị danh sách tư vấn viên
        public IActionResult Index()
        {
            var consultants = _context.Consultants.ToList();
            return View(consultants);
        }

        // Action lọc + tìm kiếm (dùng cho Ajax)
        public IActionResult FilterConsultants(string keyword, string expertise)
        {
            var query = _context.Consultants.AsQueryable();

            if (!string.IsNullOrEmpty(keyword))
            {
                keyword = keyword.ToLower();
                query = query.Where(c => c.Name.ToLower().Contains(keyword));
            }

            if (!string.IsNullOrEmpty(expertise))
            {
                query = query.Where(c => c.Expertise == expertise);
            }

            var filtered = query.ToList();
            return PartialView("_ConsultantList", filtered);
        }

        //-------------------------------------------------------------------------------------
        // Nhắn tin (Người dùng)
        // Hiển thị trang nhắn tin

        public IActionResult Chat(int id)
        {
            Console.WriteLine("===> ID nhận vào: " + id);

            var userIdStr = HttpContext.Session.GetString("UserId");
            if (userIdStr == null)
                return RedirectToAction("Login", "Account");

            var consultant = _context.Consultants.FirstOrDefault(c => c.Id == id);
            if (consultant == null)
                return Content("Không tìm thấy tư vấn viên.");

            int userId = int.Parse(userIdStr);
            ViewBag.Consultant = consultant;

            var messages = _context.Messages
                .Where(m =>
                    (m.SenderId == userId && m.SenderRole == "User" && m.ReceiverId == id && m.ReceiverRole == "Consultant") ||
                    (m.SenderId == id && m.SenderRole == "Consultant" && m.ReceiverId == userId && m.ReceiverRole == "User"))
                .OrderBy(m => m.Timestamp)
                .ToList();

            ViewBag.CurrentRole = "User";
            ViewBag.OtherPerson = consultant;
            return View(messages);

        }

        [HttpPost]
        public IActionResult SendMessage(int consultantId, string content)
        {
            int userId = int.Parse(HttpContext.Session.GetString("UserId"));

            var message = new Message
            {
                SenderId = userId,
                SenderRole = "User",
                ReceiverId = consultantId,
                ReceiverRole = "Consultant",
                Content = content,
                Timestamp = DateTime.Now
            };

            _context.Messages.Add(message);
            _context.SaveChanges();

            // ❗ Nếu Chat(int id), cần dùng "id" ở đây
            return RedirectToAction("Chat", "Consultant", new { id = consultantId });

        }
        public IActionResult LoadMessages(int consultantId)
        {
            if (HttpContext.Session.GetString("UserId") == null)
                return Unauthorized();

            int userId = int.Parse(HttpContext.Session.GetString("UserId"));

            var messages = _context.Messages
                .Where(m =>
                    (m.SenderId == userId && m.SenderRole == "User" && m.ReceiverId == consultantId && m.ReceiverRole == "Consultant") ||
                    (m.SenderId == consultantId && m.SenderRole == "Consultant" && m.ReceiverId == userId && m.ReceiverRole == "User"))
                .OrderBy(m => m.Timestamp)
                .ToList();

            ViewBag.ConsultantId = consultantId;

            return PartialView("_MessagesPartial", messages);
        }


        //------------------------------------------------------------------------
        //Nhắn tin (Nhân viên )

        //Danh sách người nhắn tin cho bạn (NV)
        public IActionResult MessagesOverview()
        {
            var consultantIdStr = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(consultantIdStr))
                return RedirectToAction("Login", "Account");

            int consultantId = int.Parse(consultantIdStr);

            // Lấy thông tin nhân viên tư vấn hiện tại
            var consultant = _context.Consultants.FirstOrDefault(c => c.Id == consultantId);
            if (consultant == null)
                return NotFound("Không tìm thấy nhân viên tư vấn.");

            var grouped = _context.Messages
                .Where(m =>
                    (m.ReceiverId == consultantId && m.ReceiverRole == "Consultant") ||
                    (m.SenderId == consultantId && m.SenderRole == "Consultant"))
                .GroupBy(m => m.SenderRole == "User" ? m.SenderId : m.ReceiverId)
                .Select(g => new
                {
                    UserId = g.Key,
                    LastMessage = g.OrderByDescending(m => m.Timestamp).FirstOrDefault()
                })
                .ToList();

            var userMessages = grouped.Select(g =>
            {
                var user = _context.Users.FirstOrDefault(u => u.Id == g.UserId);
                string direction;

                if (g.LastMessage.SenderRole == "User")
                    direction = $"{user.Username} ➝ {consultant.Name}";
                else
                    direction = $"{consultant.Name} ➝ {user.Username}";

                return new MessageOverviewViewModel
                {
                    User = user,
                    LastMessage = g.LastMessage,
                    Direction = direction
                };
            }).ToList();

            return View(userMessages);
        }


        // Trang tin nhắn với user
        public IActionResult ChatWithUser(int userId)
        {
            if (HttpContext.Session.GetString("Role") != "Consultant")
                return Unauthorized();

            int consultantId = int.Parse(HttpContext.Session.GetString("UserId"));
            var user = _context.Users.FirstOrDefault(u => u.Id == userId);
            if (user == null) return NotFound("Không tìm thấy người dùng.");

            ViewBag.User = user;

            var messages = _context.Messages
                .Where(m =>
                    (m.SenderId == consultantId && m.SenderRole == "Consultant" && m.ReceiverId == userId && m.ReceiverRole == "User") ||
                    (m.SenderId == userId && m.SenderRole == "User" && m.ReceiverId == consultantId && m.ReceiverRole == "Consultant"))
                .OrderBy(m => m.Timestamp)
                .ToList();

            ViewBag.CurrentRole = "Consultant";
            ViewBag.OtherPerson = user;
            return View(messages);

        }
        //Chức năng gửi tin nhắn
        [HttpPost]
        public IActionResult SendMessageToUser(int userId, string content)
        {
            int consultantId = int.Parse(HttpContext.Session.GetString("UserId"));
            var message = new Message
            {
                SenderId = consultantId,
                SenderRole = "Consultant",
                ReceiverId = userId,
                ReceiverRole = "User",
                Content = content,
                Timestamp = DateTime.Now
            };

            _context.Messages.Add(message);
            _context.SaveChanges();

            return RedirectToAction("ChatWithUser", new { userId });
        }
        //Chức năng cập nhật lại tin nhắn 
        public IActionResult LoadMessagesToUser(int userId)
        {
            if (HttpContext.Session.GetString("UserId") == null || HttpContext.Session.GetString("Role") != "Consultant")
                return Unauthorized();

            int consultantId = int.Parse(HttpContext.Session.GetString("UserId"));

            var user = _context.Users.FirstOrDefault(u => u.Id == userId);
            if (user == null) return NotFound("Không tìm thấy người dùng.");

            var messages = _context.Messages
                .Where(m =>
                    (m.SenderId == consultantId && m.SenderRole == "Consultant" && m.ReceiverId == userId && m.ReceiverRole == "User") ||
                    (m.SenderId == userId && m.SenderRole == "User" && m.ReceiverId == consultantId && m.ReceiverRole == "Consultant"))
                .OrderBy(m => m.Timestamp)
                .ToList();

            ViewBag.User = user;
            return PartialView("_MessagesToUserPartial", messages);
        }
        //Chức năng tìm kiếm và lọc theo thời gian gửi tin nhắn cuối cùng (trang tin nhắn từ người dùng) 
        public IActionResult GetMessagesOverview(string search, string sort = "desc")
        {
            var consultantIdStr = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(consultantIdStr)) return Unauthorized();

            int consultantId = int.Parse(consultantIdStr);

            var grouped = _context.Messages
                .Where(m => m.ReceiverId == consultantId && m.ReceiverRole == "Consultant")
                .GroupBy(m => m.SenderId)
                .Select(g => new
                {
                    UserId = g.Key,
                    LastMessage = g.OrderByDescending(m => m.Timestamp).FirstOrDefault()
                })
                .ToList();

            var userMessages = grouped.Select(g =>
            {
                var user = _context.Users.FirstOrDefault(u => u.Id == g.UserId);
                var direction = g.LastMessage.SenderRole == "User" ? $"{user.Username} ➝ NV" : $"NV ➝ {user.Username}";

                return new MessageOverviewViewModel
                {
                    User = user,
                    LastMessage = g.LastMessage,
                    Direction = direction
                };
            }).ToList();

            // Lọc theo tên
            if (!string.IsNullOrEmpty(search))
            {
                search = search.ToLower();
                userMessages = userMessages
                    .Where(m => m.User.Username.ToLower().Contains(search))
                    .ToList();
            }

            // Sắp xếp
            if (sort == "asc")
                userMessages = userMessages.OrderBy(m => m.LastMessage.Timestamp).ToList();
            else
                userMessages = userMessages.OrderByDescending(m => m.LastMessage.Timestamp).ToList();

            return PartialView("_MessagesOverviewPartial", userMessages);
        }

       
    }
}
