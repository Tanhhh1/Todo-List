using System;
using System.Collections.Generic;

namespace Todo_List.Models;

public partial class User
{
    public int Id { get; set; }

    public string Username { get; set; } = null!;

    public string? Fullname { get; set; }

    public string Email { get; set; } = null!;

    public string Password { get; set; } = null!;

    public string? Address { get; set; }

    public string? Phone { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public string? Avatar { get; set; }

    public virtual ICollection<Task> Tasks { get; set; } = new List<Task>();
}
