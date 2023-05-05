using System;
using System.ComponentModel.DataAnnotations;

namespace University.Infrasructure.Models;

public class CourseModel
{
    public int Id { get; set; }

    [Required]
    public string? Name { get; set; }

    public string? Description { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

}
