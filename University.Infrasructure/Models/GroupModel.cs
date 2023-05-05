using System;
using System.ComponentModel.DataAnnotations;

namespace University.Infrasructure.Models;

public class GroupModel
{
    [Required]
    public int Id { get; set; }

    [Required]
    public int? CourseID { get; set; }

    [Required]
    public string? Name { get; set; }
    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }
}
