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

namespace University.Tests.ServicesTests;

[TestFixture]
public class GroupServiceTests
{
    private List<Group> _testlistGroups = new();
    private Mock<IRepository<Group>> _mockRepository = new();
    private IMapper _mapper;        
    private IGroupService _groupService;
    private List<Course> _testlistCourses = new();
    private GroupModel _groupModel;


    [SetUp]
    public void Setup()
    {
        var config = new MapperConfiguration(config => config.AddMaps(Assembly.Load("University.Infrasructure")));
        _mapper = config.CreateMapper();

        _groupModel = new GroupModel();


        _testlistCourses.Clear();
        for (int i = 1; i <= 2; i++)
        {
            _testlistCourses.Add(new Course { Id = i, Name = "TestCourse {i}", Description = "CourseDesc {i}" });
        }
           
        _testlistGroups.Clear();            
        _groupService = new GroupService(_mockRepository.Object, _mapper);

        for (int i = 1; i <= 3000; i++)
        {                
            _testlistGroups.Add(new Group { Id = i, Name = $"EntityGroupName{i}", CourseID = i <= 1500 ? _testlistCourses[0].Id : _testlistCourses[1].Id });
        }            
    }

    [Test]
    public void ListEntities_ReturnsLastPage()
    {
        _mockRepository.Setup(m => m.GetAll()).Returns(_testlistGroups);
        _mockRepository.Setup(m => m.GetPaged(It.IsAny<int>(), It.IsAny<int>())).Returns((int s, int t) => _testlistGroups.Skip(s).Take(t));
        _mockRepository.Setup(m => m.Count()).Returns(_testlistGroups.Count);

        // Act
        var models = _groupService.ListEntities(2500, 1000);

        // Assert
        models.Count().Should().Be(500);
    }

    [Test]
    public void ListEntities_Returbs_correct_pages_count_with_ParentCourse()
    {

        _mockRepository.Setup(m => m.GetAll()).Returns(_testlistGroups);
        _mockRepository.Setup(m => m.GetFilteredAndPaged(It.IsAny<Expression<Func<Group, bool>>>(), It.IsAny<int>(), It.IsAny<int>()))
            .Returns((Expression<Func<Group, bool>> filter, int t, int s) => _testlistGroups.Where<Group>(filter.Compile()).Skip(s).Take(t));
        _mockRepository.Setup(m => m.Count()).Returns(_testlistGroups.Count);

        // Act
        var models = _groupService.ListEntities(_testlistCourses[0].Id, 1000, 500);

        // Assert
        models.Count().Should().Be(500);
    }

    [Test]
    public void ListEntities_ReturnsCorrectModels()
    {
        _mockRepository.Setup(m => m.GetAll()).Returns(_testlistGroups);
        var models = _groupService.ListEntities(1);

        models.Count().Should().Be(1500);
        models.Count().Should().BeGreaterThan(1499);
    }

    [Test]
    public void GetAllGroups_ReturnsCorrectModels()
    {
        _mockRepository.Setup(m => m.GetAll()).Returns(_testlistGroups);
        var models = _groupService.GetAllGroups();

        models.Count().Should().Be(3000);
        models.Count().Should().BeGreaterThan(2999);
    }

    [Test]
    public void Count_ReturnsCorrectCountWhenCourseIdIsNotNull()
    {
        _mockRepository.Setup(m => m.Count(It.IsAny<Expression<Func<Group, bool>>>()))
            .Returns(1500);

        var models = _groupService.Count(2);

        models.Should().Be(1500);
        models.Should().BeGreaterThan(1499);
    }

    [Test]
    public void InfoGroup_ReturnsCorrectGroupModel()
    {
        var groupId = 2999;
        var expectedGroup = _testlistGroups.FirstOrDefault(x => x.Id == groupId);
        if (expectedGroup != null) _mockRepository.Setup(n => n.FindById(It.IsAny<int>())).Returns(expectedGroup);

        var oneGroupModel = _groupService.InfoGroup(groupId);

        oneGroupModel.Id.Should().Be(groupId);

    }

    [Test]
    public void SaveGroup_AddNewGroup_ShouldAddToRepository1()
    {
        // Arrange
        Group? addedGroup = null;
        _ = _mockRepository.Setup(x => x.FindById(It.IsAny<int>())).Returns(((Group?)null)!);
        _mockRepository.Setup(x => x.Add(It.IsAny<Group>())).Callback((Group group) => addedGroup = group);
        var testGroup = new GroupModel { Id = 0, Name = "Test Group", CourseID = 1 };
        var groupService = new GroupService(_mockRepository.Object, _mapper);

        // Act
        groupService.SaveGroup(testGroup);

        // Assert
        _mockRepository.Verify(x => x.Add(It.IsAny<Group>()), Times.Once);
        _mockRepository.Verify(x => x.Update(It.IsAny<Group>()), Times.Never);
        addedGroup.Should().NotBeNull();
        if (addedGroup != null)
        {
            addedGroup.Name.Should().BeEquivalentTo(testGroup.Name);
            addedGroup.CourseID.Should().Be(testGroup.CourseID);
        }
    }

    [Test]
    public void SaveGroup_GroupExists_ShouldCallUpdateMethod()
    {
        // Arrange
        var groupId = 1;
        var existingGroup = new Group { Id = groupId, Name = "Existing Group", CourseID = 2 };
        _mockRepository.Setup(x => x.FindById(groupId)).Returns(existingGroup);
        var testGroup = new GroupModel { Id = groupId, Name = "Test Group", CourseID = 1 };
        Group? updatedGroup = null;

        _mockRepository.Setup(x => x.Update(It.IsAny<Group>())).Callback((Group group) => updatedGroup = group);
        var groupService = new GroupService(_mockRepository.Object, _mapper);

        // Act
        groupService.SaveGroup(testGroup);

        // Assert
        _mockRepository.Verify(x => x.Update(It.IsAny<Group>()), Times.Once);
        if (updatedGroup != null)
        {
            updatedGroup.Id.Should().Be(testGroup.Id);
            updatedGroup.Name.Should().BeEquivalentTo(testGroup.Name);
            updatedGroup.CourseID.Should().Be(testGroup.CourseID);
        }
    }
}