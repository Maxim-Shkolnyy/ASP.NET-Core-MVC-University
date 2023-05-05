using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using University.Domain.Entities;
using University.Domain.Presistent;
using University.Infrasructure.Repositories;

namespace University.Tests.RepositoryTests;

[TestFixture]
public class GroupsRepositoryTests
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
            context.Groups.RemoveRange(context.Groups);
            context.Courses.RemoveRange(context.Courses);
            context.SaveChanges();

            for (int i = 1; i <= 2; i++)
            {
                context.Courses.Add(new Course { Id = i, Name = $"EntityCourseName{i}", Description = $"EntityCourseDesc{i}" });
            }


            for (int i = 1; i <= 3000; i++)
            {
                if (i % 2 != 0)
                {
                    context.Groups.Add(new Group { Id = i, Name = $"Group {i}", CourseID = 1 });
                    continue;
                }

                context.Groups.Add(new Group { Id = i, Name = $"Group {i}", CourseID = 2 });
            }

            context.SaveChanges();
        }
    }

    [Test]
    public void Count_ReturnsCorrectCount()
    {
        using (var context = new UniversityDbContext(_dbContextOptions))
        {
            context.Groups.Count().Should().Be(3000);
        }
    }

    [Test]
    public void Add_AddCourseAfterExisting3000_3001CoursesSummaryInDatabase()
    {
        using (var context = new UniversityDbContext(_dbContextOptions))
        {
            int lastGroupId = context.Groups.Max(x => x.Id);

            context.Groups.Add(new Group
            {
                Id = ++lastGroupId,
                Name = $"EntityCourseName{++lastGroupId}",
                CourseID = ++lastGroupId
            });
            context.SaveChanges();

            int updatedCoursesCount = context.Groups.Count();

            updatedCoursesCount.Should().Be(3001);
        }
    }

    [Test]

    public void DeleteById_Delete1Course_2999CoursesSummaryInDatabase()
    {
        using (var context = new UniversityDbContext(_dbContextOptions))
        {
            var repoGroup = new GroupRepository(context);

            repoGroup.DeleteById(3000);
            context.SaveChanges();

            context.Groups.Count().Should().Be(2999);
        }
    }


    [Test]
    public void FindById_CorrectFinding()
    {
        using (var context = new UniversityDbContext(_dbContextOptions))
        {
            // Arrange
            var repository = new GroupRepository(context);
            int setCourseId = 1234;
            var expectedGroup = context.Groups.Find(setCourseId);

            var actualCourse = repository.FindById(setCourseId);

            actualCourse.Should().BeEquivalentTo(expectedGroup);
        }
    }


    [Test]
    public void GetFiltered_ReturnFilteredGroups()
    {
        using (var context = new UniversityDbContext(_dbContextOptions))
        {
            var repository = new GroupRepository(context);
            Expression<Func<Group, bool>> filter = group => group.Name != null && group.Name.Contains("1000");
            var filteredGroups = repository.GetFiltered(filter);

            filteredGroups.Should().HaveCount(1);
            filteredGroups.First().Name.Should().Be("Group 1000");
        }
    }

    [Test]
    public void GetFilteredAndPaged_ReturnFilteredGroups()
    {
        using (var context = new UniversityDbContext(_dbContextOptions))
        {
            var repo = new GroupRepository(context);

            // Act
            var filteredGroups = repo.GetFilteredAndPaged(x => x.CourseID < 3, 8, 8);

            // Assert
            filteredGroups.Should().HaveCount(8);
            filteredGroups.First().Name.Should().Be("Group 9");
            filteredGroups.Last().Name.Should().Be("Group 16");

        }
    }
}