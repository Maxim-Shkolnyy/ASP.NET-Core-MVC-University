using Microsoft.EntityFrameworkCore;
using University.Domain.Entities;

namespace University.Domain.Presistent;

public class UniversityDbContext : DbContext
{
    public UniversityDbContext(DbContextOptions<UniversityDbContext> options)
        : base(options) { }        

    public DbSet<Course> Courses { get; set; }

    public DbSet<Group> Groups { get; set; }

    public DbSet<Student> Students { get; set; }

}
