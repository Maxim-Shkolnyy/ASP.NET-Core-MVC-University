using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace University.Domain.Entities;

[Table("Students")]
public class Student : Entity
{
    public new int Id { get; set; }

    [Required]
    [Column("FIRST_NAME")]
    public string? FirstName { get; set; }

    [Required]
    [Column("LAST_NAME")]
    public string? LastName { get; set; }

    [Column("GROUP_ID")]
    public int? GroupID { get; set; }
}
