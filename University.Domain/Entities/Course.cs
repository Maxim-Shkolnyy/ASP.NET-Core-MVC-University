using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace University.Domain.Entities;

[Table("Courses")]
public class Course : Entity
{
    public new int Id { get; set; }

    [Required]
    public string? Name { get; set; }

    [Column("DESCRIPTION")]
    public string? Description { get; set; }

}
