using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Todo_List.Models;

public partial class Task
{
    public int Id { get; set; }

    public int UserId { get; set; }

    [Required(ErrorMessage = "Tiêu đề không được để trống.")]
    public string Title { get; set; } = null!;

    public string? Description { get; set; }

    public string? Status { get; set; }

    [Required(ErrorMessage = "Ngày đến hạn không được để trống.")]
    public DateTime? DueDate { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual User? User { get; set; }
}
