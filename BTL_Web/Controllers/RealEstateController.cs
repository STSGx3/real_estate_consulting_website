using BTL_Web.Data;
using Microsoft.AspNetCore.Mvc;
using BTL_Web.Helpers;
using BTL_Web.Models;

namespace BTL_Web.Controllers
{
    public class RealEstateController : Controller
    {
        private readonly ApplicationDbContext _context;

        public RealEstateController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Trang danh sách tất cả bất động sản
        public IActionResult Index()
        {
           
            var list = _context.RealEstates.ToList(); // List bất động sản
            return View(list);
        }

        // Trang chi tiết bất động sản
        public IActionResult Details(int id)
        {
            var estate = _context.RealEstates.FirstOrDefault(x => x.Id == id);

            if (estate == null)
            {
                return NotFound();
            }

            // Tính giá mỗi m² 
            var pricePerSquareMeter = estate.Price / (decimal)estate.Area;

            // Chuyển số lớn thành dạng rút gọn theo đơn vị tiền tệ 
            ViewData["PricePerSquareMeterFormatted"] = MoneyHelper.FormatMoney(pricePerSquareMeter);

            return View(estate);
        }

        // Lấy danh sách bất động sản dưới dạng file json  
        public IActionResult GetRealEstates()
        {
            var realEstates = _context.RealEstates.ToList();
            return Json(realEstates);
        }


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
            if (!string.IsNullOrEmpty(listingType)) { 
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
        //Hỏi thi 
        public IActionResult Dashboard()
        {
            var realEstates = _context.RealEstates.ToList(); // Lấy danh sách bất động sản từ DB
            return View("Dashboard", realEstates); // Trả về view vừa tạo
        }


        [HttpPost]
        public IActionResult Create(RealEstate model)
        {
            if (ModelState.IsValid)
            {
                _context.RealEstates.Add(model);
                _context.SaveChanges();
            }

            var realEstates = _context.RealEstates.ToList();
            return PartialView("_RealEstateTableDashboard", realEstates);
        }

        [HttpGet]
        public IActionResult List()
        {
            var realEstates = _context.RealEstates.ToList();
            return PartialView("_RealEstateTableDashboard", realEstates);
        }
    }

}
