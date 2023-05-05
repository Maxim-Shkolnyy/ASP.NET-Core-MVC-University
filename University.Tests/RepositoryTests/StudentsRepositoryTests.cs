using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using University.Domain.Entities;
using University.Domain.Presistent;
using University.Infrasructure.Repositories;

namespace University.Tests.RepositoryTests;

[TestFixture]
public class StudentsRepositoryTests
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
            context.Students.RemoveRange(context.Students);
            context.Courses.RemoveRange(context.Courses);
            context.Groups.RemoveRange(context.Groups);
            context.SaveChanges();

            for (int i = 1; i <= 2; i++)
            {
                context.Courses.Add(new Course { Id = i, Name = $"EntityCourseName{i}", Description = $"EntityCourseDesc{i}" });
            }

            for (int i = 1; i <= 20; i++)
            {
                if (i % 2 != 0)
                {
                    context.Groups.Add(new Group { Id = i, Name = $"Group {i}", CourseID = 1 });
                    continue;
                }

                context.Groups.Add(new Group { Id = i, Name = $"Group {i}", CourseID = 2 });
            }

            int groupId = 1;
            for (int i = 1; i <= 3000; i++)
            {
                if (groupId > 20)
                {
                    groupId = 1;
                }
                context.Students.Add(new Student { Id = i, FirstName = $"FirstName {i}", LastName = $"LastName {i}", GroupID = groupId });
                groupId++;
            }

            context.SaveChanges();
        }
    }


    [Test]
    public void Count_ReturnsCorrectCount()
    {
        using (var context = new UniversityDbContext(_dbContextOptions))
        {
            context.Students.Count().Should().Be(3000);
        }
    }

    [Test]
    public void Add_AddStudentAfterExisting3000_3001CoursesSummaryInDatabase()
    {
        using (var context = new UniversityDbContext(_dbContextOptions))
        {
            int lastStudentsId = context.Students.Max(x => x.Id);

            context.Students.Add(new Student
            {
                Id = ++lastStudentsId,
                FirstName = $"FirstName{++lastStudentsId}",
                LastName = $"LastName{++lastStudentsId}",
                GroupID = 1
            });
            context.SaveChanges();

            int updatedStudentsCount = context.Students.Count();

            updatedStudentsCount.Should().Be(3001);
        }
    }

    [Test]

    public void DeleteById_Delete1Student_2999CoursesSummaryInDatabase()
    {
        using (var context = new UniversityDbContext(_dbContextOptions))
        {
            var repoStudent = new StudentRepository(context);

            repoStudent.DeleteById(3000);
            context.SaveChanges();

            context.Students.Count().Should().Be(2999);
        }
    }


    [Test]
    public void FindById_CorrectFinding()
    {
        using (var context = new UniversityDbContext(_dbContextOptions))
        {
            var repoStudent = new StudentRepository(context);
            int setCourseId = 1234;
            var expectedGroup = context.Students.Find(setCourseId);

            var actualCourse = repoStudent.FindById(setCourseId);

            actualCourse.Should().BeEquivalentTo(expectedGroup);
        }
    }


    [Test]
    public void GetFiltered_ReturnFilteredStudents()
    {
        using (var context = new UniversityDbContext(_dbContextOptions))
        {
            var repoStudent = new StudentRepository(context);
            Expression<Func<Student, bool>> filter = student => student.FirstName != null && student.FirstName.Contains("1000");
            var filteredGroups = repoStudent.GetFiltered(filter);

            filteredGroups.Should().HaveCount(1);
            filteredGroups.First().FirstName.Should().Be("FirstName 1000");
        }
    }

    [Test]
    public void GetFilteredAndPaged_ReturnFilteredStudents()
    {
        using (var context = new UniversityDbContext(_dbContextOptions))
        {
            var repo = new StudentRepository(context);

            // Act
            var filteredGroups = repo.GetFilteredAndPaged(x => x.GroupID < 21, 8, 8);

            // Assert
            filteredGroups.Should().HaveCount(8);
            filteredGroups.First().FirstName.Should().Be("FirstName 9");
            filteredGroups.Last().FirstName.Should().Be("FirstName 16");
        }
    }
}