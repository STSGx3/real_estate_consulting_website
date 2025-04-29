using System.ComponentModel.DataAnnotations;
namespace BTL_Web.Models
{
    using System.ComponentModel.DataAnnotations;
    // mode bất động sản
    public class RealEstate
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Tên bất động sản là bắt buộc")]
        [StringLength(100, ErrorMessage = "Tên không được vượt quá 100 ký tự")]
        public string Name { get; set; }

        [Required]
        public string Location { get; set; }

        public string Description { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Giá phải là số dương")]
        public decimal Price { get; set; }

        public string ImageUrl { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Giá phải là số dương")]
        public double Area { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn loại bất động sản")]
        public string Type { get; set; } 

        public string ListingType { get; set; } // "Cho thuê" hoặc "Mua bán"
        public int BedroomCount { get; set; }
        public int BathroomCount { get; set; }
    }


}
