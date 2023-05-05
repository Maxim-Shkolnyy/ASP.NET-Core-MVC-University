using AutoMapper;
using FluentAssertions;
using Moq;
using System.Linq.Expressions;
using System.Reflection;
using University.Domain.Contracts;
using University.Domain.Entities;
using University.Infrasructure.Models;
using University.Infrasructure.Services;
using Group = University.Domain.Entities.Group;
using Microsoft.CodeAnalysis;

namespace University.Tests.ServicesTests;

[TestFixture]
public class StudentsServiceTest
{
    private List<Course> _testlistCourses = new();
    private List<Group> _testlistGroups = new();
    private List<Student> _testlistStudents = new();
    private Mock<IRepository<Student>> _mockStudentRepository = new();
    private Mock<IRepository<Group>> _mockGroupRepository = new();

    private IMapper _mapper;
    //private ICourseService _courseService;
    private IGroupService _groupService;
    private IStudentService _studentService;
    private StudentModel _studentModel;


    [SetUp]
    public void Setup()
    {
        var config = new MapperConfiguration(config => config.AddMaps(Assembly.Load("University.Infrasructure")));
        _mapper = config.CreateMapper();

        _studentModel = new StudentModel();


        _testlistCourses.Clear();

        _testlistCourses.Add(new Course { Id = 1, Name = "TestCourse 1", Description = "CourseDesc 1" });


        _testlistGroups.Clear();
        _groupService = new GroupService(_mockGroupRepository.Object, _mapper);

        _testlistGroups.Add(new Group { Id = 1, Name = $"EntityGroupName1", CourseID = 1 });
        _testlistGroups.Add(new Group { Id = 2, Name = $"EntityGroupName2", CourseID = 1 });

        _testlistStudents.Clear();
        _studentService = new StudentService(_mockStudentRepository.Object, _mapper);
        for (int i = 1; i <= 3000; i++)
        {
            _testlistStudents.Add(new Student { Id = i, FirstName = $"FName{i}", LastName = $"LName {i}", GroupID = i <= 1500 ? 1 : 2 });
        }
    }
         
    [Test]
    public void ListEntities_ReturnsLastPage()
    {
        _mockStudentRepository.Setup(m => m.GetAll()).Returns(_testlistStudents);
        _mockStudentRepository.Setup(m => m.GetPaged(It.IsAny<int>(), It.IsAny<int>())).Returns((int s, int t) => _testlistStudents.Skip(s).Take(t));
        _mockStudentRepository.Setup(m => m.GetFilteredAndPaged(It.IsAny<Expression<Func<Student, bool>>>(), It.IsAny<int>(), It.IsAny<int>()))
            .Returns((Expression<Func<Student, bool>> filter, int t, int s) => _testlistStudents.Where<Student>(filter.Compile()).Skip(s).Take(t));
        _mockStudentRepository.Setup(m => m.Count()).Returns(_testlistStudents.Count);

        // Act
        var models = _studentService.ListEntities(_testlistStudents[1501].GroupID, 2500, 1000);

        // Assert
        models.Count().Should().Be(500);
        models.Last().Should().NotBeNull();
        models.Last().Id.Should().Be(3000);
    }

    [Test]
    public void ListEntities_ParentGroup()
    {

        _mockStudentRepository.Setup(m => m.GetAll()).Returns(_testlistStudents);
        _mockStudentRepository.Setup(m => m.GetFilteredAndPaged(It.IsAny<Expression<Func<Student, bool>>>(), It.IsAny<int>(), It.IsAny<int>()))
            .Returns((Expression<Func<Student, bool>> filter, int t, int s) => _testlistStudents.Where<Student>(filter.Compile()).Take(t).Skip(s));
        _mockStudentRepository.Setup(m => m.Count()).Returns(_testlistStudents.Count);

        // Act
        var models = _studentService.ListEntities(_testlistStudents[0].Id, 1000, 500);

        // Assert
        models.Count().Should().Be(500);
    }

    [Test]
    public void ListEntities_ReturnsCorrectModels()
    {
        _mockStudentRepository.Setup(m => m.GetAll()).Returns(_testlistStudents);
        var models = _studentService.ListEntities(1);

        models.Count().Should().Be(1500);
        models.Count().Should().BeGreaterThan(1499);
    }

    [Test]
    public void GetAllStudents_ReturnsCorrectModels()
    {
        _mockStudentRepository.Setup(m => m.GetAll()).Returns(_testlistStudents);
        var models = _studentService.GetAllStudents();

        models.Count().Should().Be(3000);            
    }

    [Test]
    public void Count_ReturnsCorrectCountWhenGroupIdIsNotNull()
    {
        _mockStudentRepository.Setup(m => m.Count(It.IsAny<Expression<Func<Student, bool>>>()))
            .Returns(1500);

        var models = _studentService.Count(2);

        models.Should().Be(1500);
        models.Should().BeGreaterThan(1499);
    }

    [Test]
    public void InfoGroup_ReturnsCorrectStudentModel()
    {
        var studentId = 2999;
        var expectedStudent = _testlistStudents.FirstOrDefault(x => x.Id == studentId);
        if (expectedStudent != null)
            _mockStudentRepository.Setup(n => n.FindById(It.IsAny<int>())).Returns(expectedStudent);

        var oneStudentModel = _studentService.InfoStudent(studentId);

        oneStudentModel.Id.Should().Be(studentId);

    }


    [Test]
    public void SaveStudent_AddNewGroup_ShouldAddToRepository1()
    {
        // Arrange
        Student? addedStudent = null;
        _ = _mockStudentRepository.Setup(x => x.FindById(It.IsAny<int>())).Returns(((Student?)null)!);
        _mockStudentRepository.Setup(x => x.Add(It.IsAny<Student>())).Callback((Student? student) => addedStudent = student);
        var testGroup = new StudentModel { Id = 0, FirstName = "Test Group", GroupID = 1 };
        var groupService = new StudentService(_mockStudentRepository.Object, _mapper);

        // Act
        groupService.SaveStudent(testGroup);

        // Assert
        _mockStudentRepository.Verify(x => x.Add(It.IsAny<Student>()), Times.Once);
        _mockStudentRepository.Verify(x => x.Update(It.IsAny<Student>()), Times.Never);
        addedStudent.Should().NotBeNull();
        if (addedStudent != null)
        {
            addedStudent.FirstName.Should().BeEquivalentTo(testGroup.FirstName);
            addedStudent.GroupID.Should().Be(testGroup.GroupID);
        }
    }


    [Test]
    public void SaveStudent_GroupExists_ShouldCallUpdateMethod()
    {
        // Arrange
        var studentId = 1;
        var existingStudent = new Student { Id = studentId, FirstName = "Existing Group", GroupID = 2 };
        _mockStudentRepository.Setup(x => x.FindById(studentId)).Returns(existingStudent);
        var testStudent = new StudentModel { Id = studentId, FirstName = "Test Group", GroupID = 1 };
        Student? updatedStudent = null;

        _mockStudentRepository.Setup(x => x.Update(It.IsAny<Student>())).Callback((Student? student) => updatedStudent = student);
        var groupService = new StudentService(_mockStudentRepository.Object, _mapper);

        // Act
        groupService.SaveStudent(testStudent);

        // Assert
        _mockStudentRepository.Verify(x => x.Update(It.IsAny<Student>()), Times.Once);
        if (updatedStudent != null)
        {
            updatedStudent.Id.Should().Be(testStudent.Id);
            updatedStudent.FirstName.Should().BeEquivalentTo(testStudent.FirstName);
            updatedStudent.GroupID.Should().Be(testStudent.GroupID);
        }
    }
}