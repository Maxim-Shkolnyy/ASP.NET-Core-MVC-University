using System;
using System.ComponentModel.DataAnnotations;

namespace University.Infrasructure.Models;

public class StudentModel
{
    [Required]
    public int Id { get; set; }

    [Required]
    public string? FirstName { get; set; }

    public string? LastName { get; set; }

    [Required]
    public int? GroupID { get; set; }
    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }
}
