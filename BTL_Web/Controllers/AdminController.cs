using Microsoft.AspNetCore.Mvc;
using System.Linq;
using BTL_Web.Models; 
using BTL_Web.Data;   


public class AdminController : Controller
{
    private readonly ApplicationDbContext _context;

    public AdminController(ApplicationDbContext context)
    {
        _context = context;
    }

    public IActionResult Dashboard()
    {
        var role = HttpContext.Session.GetString("Role");
        if (role != "Admin") return Unauthorized();

        ViewBag.Username = HttpContext.Session.GetString("Username");
        return View();
    }

    //------------------------------------------------------------------------------------------------------
    //Quản lý người dùng 
    public IActionResult UserList()
    {
        var users = _context.Users
                       .Where(u => u.Role != "Admin") 
                       .ToList();
        return View(users);
    }

    [HttpPost]
    public IActionResult DeleteUser(int id)
    {
        var user = _context.Users.FirstOrDefault(u => u.Id == id);
        if (user == null)
            return Json(new { success = false, message = "Không tìm thấy người dùng" });

        _context.Users.Remove(user);
        _context.SaveChanges();

        return Json(new { success = true });
    }

    [HttpPost]
    public IActionResult SearchUsers(string query)
    {
        try
        {
            var usersQuery = _context.Users.AsQueryable();

            // Bỏ qua role Admin khỏi danh sách người dùng
            usersQuery = usersQuery.Where(u => u.Role != "Admin");

           
            if (!string.IsNullOrWhiteSpace(query))
            {
                var loweredQuery = query.ToLower();
                usersQuery = usersQuery.Where(u => u.Username.ToLower().Contains(loweredQuery));
            }

            var users = usersQuery.ToList();
            return PartialView("_UserListPartial", users);
        }
        catch (Exception ex)
        {
            Console.WriteLine("Search Error: " + ex.Message);
            return StatusCode(500); // fix lỗi -_- 
        }
    }

    //--------------------------------------------------------------------------------------------------------
    //Quản lý bất động sản 

    // Hiển thị danh sách BĐS
    public IActionResult ManageRealEstates()
    {
        if (HttpContext.Session.GetString("Role") != "Admin")
            return Unauthorized();

        var estates = _context.RealEstates.ToList();
        return View(estates);
    }

    // GET: Thêm mới BĐS
    public IActionResult CreateRealEstate()
    {
        if (HttpContext.Session.GetString("Role") != "Admin")
            return Unauthorized();

        return View();
    }

    // POST: Thêm mới

    [HttpPost]
    public IActionResult CreateRealEstate(RealEstate estate)
    {
        if (ModelState.IsValid)
        {
            Console.WriteLine("Thêm bất động sản thành công!");
            _context.RealEstates.Add(estate);
            _context.SaveChanges();
            return RedirectToAction("ManageRealEstates");
        }
        else
        {
            Console.WriteLine("Có lỗi trong dữ liệu: " + string.Join(", ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage)));
        }

        return View(estate);
    }


    // GET: Sửa BĐS
    public IActionResult EditRealEstate(int id)
    {
        var estate = _context.RealEstates.Find(id);
        if (estate == null) return NotFound();

        return View(estate);
    }

    // POST: Sửa BĐS
    [HttpPost]
    public IActionResult EditRealEstate(RealEstate estate)
    {
        if (ModelState.IsValid)
        {
            _context.RealEstates.Update(estate);
            _context.SaveChanges();
            return RedirectToAction("ManageRealEstates");
        }
        return View(estate);
    }

    // Xóa BĐS
    public IActionResult DeleteRealEstate(int id)
    {
        var estate = _context.RealEstates.Find(id);
        if (estate == null) return NotFound();

        _context.RealEstates.Remove(estate);
        _context.SaveChanges();
        return RedirectToAction("ManageRealEstates");
    }

    // Tìm kiếm và lọc BĐS
    public IActionResult FilterRealEstates(string keyword, string type, string price, string area, string sortBy, string sortOrder, string listingType)
    {
        var query = _context.RealEstates.AsQueryable();

        // Tìm kiếm theo tên (không phân biệt hoa thường)
        if (!string.IsNullOrEmpty(keyword))
        {
            keyword = keyword.ToLower();
            query = query.Where(r => r.Name.ToLower().Contains(keyword));
        }

        //Lọc theo hình thức
        if (!string.IsNullOrEmpty(listingType))
        {
            query = query.Where(r => r.ListingType == listingType);
        }
        // Lọc theo loại
        if (!string.IsNullOrEmpty(type))
        {
            query = query.Where(r => r.Type == type);
        }

        // Lọc theo giá
        if (!string.IsNullOrEmpty(price))
        {
            var range = price.Split('-');
            if (range.Length == 2 &&
                decimal.TryParse(range[0], out var min) &&
                decimal.TryParse(range[1], out var max))
            {
                query = query.Where(r => r.Price >= min && r.Price <= max);
            }
        }

        // Lọc theo diện tích
        if (!string.IsNullOrEmpty(area))
        {
            var range = area.Split('-');
            if (range.Length == 2 &&
                double.TryParse(range[0], out var minArea) &&
                double.TryParse(range[1], out var maxArea))
            {
                query = query.Where(r => r.Area >= minArea && r.Area <= maxArea);
            }
        }

        // 👉 Sắp xếp theo sortBy và sortOrder
        if (!string.IsNullOrEmpty(sortBy))
        {
            bool ascending = sortOrder?.ToLower() == "asc";

            switch (sortBy.ToLower())
            {
                case "price":
                    query = ascending ? query.OrderBy(r => r.Price) : query.OrderByDescending(r => r.Price);
                    break;
                case "area":
                    query = ascending ? query.OrderBy(r => r.Area) : query.OrderByDescending(r => r.Area);
                    break;
            }
        }

        return PartialView("_RealEstateTable", query.ToList());
    }
    //----------------------------------------------------------------------------------------------
    // Quản lý nhân viên tư vấn

    // Hiển thị danh sách
    public IActionResult ManageConsultants()
    {
        if (HttpContext.Session.GetString("Role") != "Admin")
            return Unauthorized();

        var consultants = _context.Consultants.ToList();
        return View(consultants);
    }

    // GET: Thêm mới
    public IActionResult CreateConsultant()
    {
        if (HttpContext.Session.GetString("Role") != "Admin")
            return Unauthorized();

        return View();
    }

    // POST: Thêm mới
    [HttpPost]
    public IActionResult CreateConsultant(ConsultantWithAccountViewModel model)
    {
        if (ModelState.IsValid)
        {
            var consultant = new Consultant
            {
                Name = model.Name,
                Email = model.Email,
                PhoneNumber = model.PhoneNumber,
                Expertise = model.Expertise,
                ImageUrl = model.ImageUrl
            };
            _context.Consultants.Add(consultant);
            _context.SaveChanges(); // Bắt buộc

            var user = new User
            {
                Username = model.Username,
                Password = model.Password,
                Role = "Consultant"
            };
            _context.Users.Add(user);
            _context.SaveChanges();

            return RedirectToAction("ManageConsultants");
        }

        return View(model);
    }



    // GET: Sửa
    public IActionResult EditConsultant(int id)
    {
        if (HttpContext.Session.GetString("Role") != "Admin")
            return Unauthorized();

        var consultant = _context.Consultants.Find(id);
        if (consultant == null) return NotFound();

        var model = new ConsultantWithAccountViewModel
        {
            Id = consultant.Id,
            Name = consultant.Name,
            Email = consultant.Email,
            PhoneNumber = consultant.PhoneNumber,
            Expertise = consultant.Expertise,
            ImageUrl = consultant.ImageUrl
            // Không cần gán Username/Password vì chỉ sửa thông tin Consultant
        };

        return View(model);
    }


    // POST: Sửa
    [HttpPost]
    public IActionResult EditConsultant(ConsultantWithAccountViewModel model)
    {
        if (ModelState.IsValid)
        {
            var consultant = _context.Consultants.FirstOrDefault(c => c.Id == model.Id);
            if (consultant == null) return NotFound();

            consultant.Name = model.Name;
            consultant.Email = model.Email;
            consultant.PhoneNumber = model.PhoneNumber;
            consultant.Expertise = model.Expertise;
            consultant.ImageUrl = model.ImageUrl;

            _context.Consultants.Update(consultant);
            _context.SaveChanges();

            return RedirectToAction("ManageConsultants");
        }

        return View(model);
    }


    // Xóa
    public IActionResult DeleteConsultant(int id)
    {
        if (HttpContext.Session.GetString("Role") != "Admin")
            return Unauthorized();

        var consultant = _context.Consultants.Find(id);
        if (consultant == null) return NotFound();

        _context.Consultants.Remove(consultant);
        _context.SaveChanges();
        return RedirectToAction("ManageConsultants");
    }

    // Tìm kiếm và lọc theo tên và chuyên môn 
    public IActionResult FilterStaffs(string keyword, string expertis)
    {
        if (HttpContext.Session.GetString("Role") != "Admin")
            return Unauthorized();

        var staffs = _context.Consultants.AsQueryable();

        if (!string.IsNullOrEmpty(keyword))
        {
            keyword = keyword.ToLower();
            staffs = staffs.Where(s => s.Name.ToLower().Contains(keyword));
        }

        if (!string.IsNullOrEmpty(expertis))
        {
            staffs = staffs.Where(s => s.Expertise == expertis);
        }

        return PartialView("_StaffTable", staffs.ToList());
    }

    //---------------------------------------------------------------------------------------------------------
    //Quản lý tin nhắn tư vấn 

    //Danh sách các đoạn chat
    public IActionResult ManageChats()
    {
        return View(); // view chính
    }

    public IActionResult LoadChatList(string search = "", string sortOrder = "desc")
    {
        var grouped = _context.Messages
            .GroupBy(m => new { m.SenderId, m.SenderRole, m.ReceiverId, m.ReceiverRole })
            .Select(g => g.OrderByDescending(m => m.Timestamp).FirstOrDefault())
            .ToList();

        var distinctChats = grouped
            .GroupBy(m => m.SenderRole == "User"
                ? new { UserId = m.SenderId, ConsultantId = m.ReceiverId }
                : new { UserId = m.ReceiverId, ConsultantId = m.SenderId })
            .Select(g =>
            {
                var lastMessage = g.OrderByDescending(m => m.Timestamp).FirstOrDefault();
                var user = _context.Users.FirstOrDefault(u => u.Id == g.Key.UserId);
                var consultant = _context.Consultants.FirstOrDefault(c => c.Id == g.Key.ConsultantId);

                return new ChatSummaryViewModel
                {
                    UserId = g.Key.UserId,
                    Username = user?.Username,
                    ConsultantId = g.Key.ConsultantId,
                    ConsultantName = consultant?.Name,
                    LastMessageTime = lastMessage.Timestamp,
                    LastMessageContent = lastMessage.Content
                };
            })
            .ToList();

        // Tìm kiếm theo tên nhân viên
        if (!string.IsNullOrEmpty(search))
        {
            distinctChats = distinctChats
                .Where(c => !string.IsNullOrEmpty(c.ConsultantName) &&
                            c.ConsultantName.Contains(search, StringComparison.OrdinalIgnoreCase))
                .ToList();
        }

        //Sắp xếp theo thời gian
        distinctChats = sortOrder == "asc"
            ? distinctChats.OrderBy(c => c.LastMessageTime).ToList()
            : distinctChats.OrderByDescending(c => c.LastMessageTime).ToList();

        return PartialView("_ChatSummaryList", distinctChats);
    }

    public IActionResult ChatDetail(int userId, int consultantId)
    {
        var messages = _context.Messages
            .Where(m =>
                (m.SenderId == userId && m.ReceiverId == consultantId) ||
                (m.SenderId == consultantId && m.ReceiverId == userId))
            .OrderBy(m => m.Timestamp)
            .ToList();

        ViewBag.User = _context.Users.FirstOrDefault(u => u.Id == userId);
        ViewBag.Consultant = _context.Consultants.FirstOrDefault(c => c.Id == consultantId);

        return PartialView("_ChatDetail", messages);
    }

    [HttpPost]
    public IActionResult DeleteChat(int userId, int consultantId)
    {
        var messages = _context.Messages
            .Where(m =>
                (m.SenderId == userId && m.ReceiverId == consultantId) ||
                (m.SenderId == consultantId && m.ReceiverId == userId))
            .ToList();

        _context.Messages.RemoveRange(messages);
        _context.SaveChanges();

        return Ok();
    }


}
