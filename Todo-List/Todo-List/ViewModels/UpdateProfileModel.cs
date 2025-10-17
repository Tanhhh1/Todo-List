using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace Todo_List.ViewModels
{
    public class UpdateProfileModel
    {
        public int Id { get; set; }
        public string Username { get; set; } = null!;
        public string? Fullname { get; set; }

        [Required(ErrorMessage = "Email không được để trống")]
        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
        public string Email { get; set; } = null!;

        public string? Address { get; set; }

        [RegularExpression(@"^0\d{9,10}$", ErrorMessage = "Số điện thoại không hợp lệ")]
        public string? Phone { get; set; }

        public string? Avatar { get; set; } 
    }
}
