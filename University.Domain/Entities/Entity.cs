using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace University.Domain.Entities;

public class Entity
{
    [Key]
    [Required]
    public int Id { get; set; }

    [Column("CREATE_AT")]
    public DateTime CreatedAt {get; set;}

    [Column("UPDATE_AT")]
    public DateTime? UpdatedAt { get; set;}        

}
