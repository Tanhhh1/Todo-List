using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Todo_List.Models
{
    public partial class User
    {
        public int Id { get; set; }

        public string Username { get; set; } = null!;

        public string? Fullname { get; set; }

        public string Email { get; set; } = null!;

        public string Password { get; set; } = null!;

        [NotMapped]
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
