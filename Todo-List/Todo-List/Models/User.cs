using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Todo_List.Models
{
    public partial class User
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập tên đăng nhập.")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Tên đăng nhập phải từ 3 đến 50 ký tự.")]
        public string Username { get; set; } = null!;

        public string? Fullname { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập email.")]
        [EmailAddress(ErrorMessage = "Định dạng email không hợp lệ.")]
        public string Email { get; set; } = null!;

        [Required(ErrorMessage = "Vui lòng nhập mật khẩu.")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Mật khẩu phải có ít nhất 6 ký tự.")]
        public string Password { get; set; } = null!;

        [NotMapped]
        [Compare("Password", ErrorMessage = "Mật khẩu xác nhận không khớp.")]
        [Required(ErrorMessage = "Vui lòng nhập xác nhận mật khẩu.")]
        public string ConfirmPassword { get; set; } = null!;

        public string? Address { get; set; }

        public DateOnly? Birthday { get; set; }

        public string? Phone { get; set; }

        public DateTime? CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }

        public string? Avatar { get; set; }

        public virtual ICollection<Task> Tasks { get; set; } = new List<Task>();
    }
}
