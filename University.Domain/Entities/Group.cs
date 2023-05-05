using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace University.Domain.Entities;

[Table("Groups")]
public class Group : Entity
{
    public new int Id { get; set; }

    [Required]
    [Column("COURSE_ID")]
    public int CourseID { get; set; }

    [Required]
    [Column("NAME")]
    public string? Name { get; set; }
  
}
