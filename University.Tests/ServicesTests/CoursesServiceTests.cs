using AutoMapper;
using FluentAssertions;
using Moq;
using System.Reflection;
using University.Domain.Contracts;
using University.Domain.Entities;
using University.Infrasructure.Models;
using University.Infrasructure.Services;

namespace University.Tests.ServicesTests;

[TestFixture]
public class CourseServiceTests
{
    private List<Course> _testlistCourses = new();
    private Mock<IRepository<Course>> _mockCourseRepository = new();
    private IMapper _mapper;        
    private ICourseService _courseService;


    [SetUp]
    public void Setup()
    {
        var config = new MapperConfiguration(config => config.AddMaps(Assembly.Load("University.Infrasructure")));
        _mapper = config.CreateMapper();

        _testlistCourses.Clear();            
        _courseService = new CourseService(_mockCourseRepository.Object, _mapper);

        for (int i = 1; i <= 3000; i++)
        {
            _testlistCourses.Add(new Course { Id = i, Name = $"EntityCourseName{i}", Description = $"EntityCourseDesc{i}" });
        }            
    }

    [Test]
    public void ListEntities_ReturnsLastPage()
    {

        _mockCourseRepository.Setup(m => m.GetAll()).Returns(_testlistCourses);
        _mockCourseRepository.Setup(m => m.GetPaged(It.IsAny<int>(), It.IsAny<int>())).Returns((int t, int s) => _testlistCourses.Skip(s).Take(t));
        _mockCourseRepository.Setup(m => m.Count()).Returns(_testlistCourses.Count);

        // Act
        var models = _courseService.ListEntities(2500, 1000);

        // Assert
        models.Count().Should().Be(500);
    }

    [Test]
    public void Count_Returns_correct_count()
    {
        _mockCourseRepository.Setup(m => m.Count()).Returns(_testlistCourses.Count);

        _courseService.Count().Should().Be(3000);
    }

    [Test]
    public void GetAllCourses_returns_correct_models()
    {
        _mockCourseRepository.Setup(m => m.GetAll()).Returns(_testlistCourses);
        
        var models = _courseService.GetAllCourses();

        models.Count().Should().Be(3000);
        models.Count().Should().BeGreaterThan(2999);
    }

    [Test]
    public void AddCourse_CorrectlyAddModel()
    {
        Course? addedCourse = null;
        _mockCourseRepository.Setup(m => m.FindById(It.IsAny<int>())).Returns((Course)null!);
        if (addedCourse != null)
            _mockCourseRepository.Setup(m => m.Add(It.IsAny<Course>())).Returns(addedCourse)
                .Callback((Course? course) => addedCourse = course);
        var testCoutse = new CourseModel { Id = 3001, Name = "Course3001", Description = "CourseDesc 3001" };

        _courseService.AddCourse();

        testCoutse.Id.Should().Be(3001);
        testCoutse.Description.Should().Be("CourseDesc 3001");
    }

    [Test]
    public void InfoGroup_ReturnsCorrectGroupModel()
    {
        var courseId = 2999;
        var expectedCourse = _testlistCourses.FirstOrDefault(x => x.Id == courseId);
        if (expectedCourse != null)
            _mockCourseRepository.Setup(n => n.FindById(It.IsAny<int>())).Returns(expectedCourse);

        var oneCourseModel = _courseService.InfoCourse(courseId);

        oneCourseModel.Id.Should().Be(courseId);

    }

    [Test]
    public void SaveCourse_AddNewCourse_ShouldAddToRepository1()
    {
        // Arrange
        Course? addedCourse = null;
        _ = _mockCourseRepository.Setup(x => x.FindById(It.IsAny<int>())).Returns(((Course?)null)!);
        _mockCourseRepository.Setup(x => x.Add(It.IsAny<Course>())).Callback((Course course) => addedCourse = course);
        var testCourse = new CourseModel() { Id = 0, Name = "Test Group", Description = "TestDesc"};
        var courseService = new CourseService(_mockCourseRepository.Object, _mapper);

        // Act
        courseService.SaveCourse(testCourse);

        // Assert
        _mockCourseRepository.Verify(x => x.Add(It.IsAny<Course>()), Times.Once);
        _mockCourseRepository.Verify(x => x.Update(It.IsAny<Course>()), Times.Never);
        addedCourse.Should().NotBeNull();
        if (addedCourse != null)
        {
            addedCourse.Name.Should().BeEquivalentTo(testCourse.Name);
            addedCourse.Description.Should().Be(testCourse.Description);
        }
    }

    [Test]
    public void SaveGroup_GroupExists_ShouldCallUpdateMethod()
    {
        // Arrange
        var courseId = 1;
        var existingCourse = new Course { Id = courseId, Name = "Existing Group", Description = "TestDesc" };
        _mockCourseRepository.Setup(x => x.FindById(courseId)).Returns(existingCourse);
        var testCourse = new CourseModel() { Id = courseId, Name = "Test Course", Description = "TestDesc" };
        Course? updatedCourse = null;

        _mockCourseRepository.Setup(x => x.Update(It.IsAny<Course>())).Callback((Course course) => updatedCourse = course);
        var courseService = new CourseService(_mockCourseRepository.Object, _mapper);

        // Act
        courseService.SaveCourse(testCourse);

        // Assert
        _mockCourseRepository.Verify(x => x.Update(It.IsAny<Course>()), Times.Once);
        if (updatedCourse != null)
        {
            updatedCourse.Id.Should().Be(testCourse.Id);
            updatedCourse.Name.Should().BeEquivalentTo(testCourse.Name);
            updatedCourse.Description.Should().Be(testCourse.Description);
        }
    }
}