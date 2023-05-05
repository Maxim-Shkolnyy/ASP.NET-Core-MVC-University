using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using University.Domain.Entities;
using University.Domain.Presistent;
using University.Infrasructure.Repositories;

namespace University.Tests.RepositoryTests;

[TestFixture]
public class CoursesRepositoryTests
{

    DbContextOptions<UniversityDbContext> _dbContextOptions;

    [SetUp]
    public void Setup()
    {

        _dbContextOptions = new DbContextOptionsBuilder<UniversityDbContext>()
            .UseInMemoryDatabase(databaseName: "UniversityDb")
            .Options;

        using (var context = new UniversityDbContext(_dbContextOptions))
        {
            context.Courses.RemoveRange(context.Courses);
            context.SaveChanges();


            for (int i = 1; i <= 3000; i++)
            {
                context.Courses.Add(new Course { Id = i, Name = $"EntityCourseName{i}", Description = $"EntityCourseDesc{i}" });
            }

            context.SaveChanges();
        }
    }


    [Test]
    public void Count_ReturnsCorrectCount()
    {
        using (var context = new UniversityDbContext(_dbContextOptions))
        {
            context.Courses.Count().Should().Be(3000);
        }
    }


    [Test]
    public void Add_AddCourseAfterExisting3000_3001CoursesSummaryInDatabase()
    {
        using (var context = new UniversityDbContext(_dbContextOptions))
        {
            int coursesCount = context.Courses.Max(x => x.Id);

            context.Courses.Add(new Course
            {
                Id = ++coursesCount,
                Name = $"EntityCourseName{++coursesCount}",
                Description = $"EntityCourseDesc{++coursesCount}"
            });
            context.SaveChanges();

            int updatedCoursesCount = context.Courses.Count();

            updatedCoursesCount.Should().Be(3001);
        }
    }


    [Test]

    public void DeleteById_Delete1Course_2999CoursesSummaryInDatabase()
    {
        using (var context = new UniversityDbContext(_dbContextOptions))
        {
            var repoCourse = new CourseRepository(context);

            repoCourse.DeleteById(3000);
            context.SaveChanges();

            context.Courses.Count().Should().Be(2999);
        }
    }


    [Test]
    public void FindById_CorrectFinding()
    {
        using (var context = new UniversityDbContext(_dbContextOptions))
        {
            // Arrange
            var repository = new CourseRepository(context);
            int setCourseId = 1234;
            var expectedCourse = context.Courses.Find(setCourseId);

            var actualCourse = repository.FindById(setCourseId);

            actualCourse.Should().BeEquivalentTo(expectedCourse);
        }
    }


    [Test]
    public void GetFiltered_ReturnFilteredCourses()
    {
        using (var context = new UniversityDbContext(_dbContextOptions))
        {
            var repository = new CourseRepository(context);
            Expression<Func<Course, bool>> filter =
                course => course.Name != null && course.Name.Contains("1000");
            var filteredCourses = repository.GetFiltered(filter);

            filteredCourses.Should().HaveCount(1);
            filteredCourses.First().Name.Should().BeEquivalentTo("EntityCourseName1000");
        }
    }
}