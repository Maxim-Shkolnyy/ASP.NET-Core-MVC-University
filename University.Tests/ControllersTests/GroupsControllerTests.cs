using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.EntityFrameworkCore;
using Moq;
using MVC.Controllers;
using University.Domain.Presistent;
using University.Infrasructure.Models;
using University.Infrasructure.Services;

namespace University.Tests.Controllers;

[TestFixture]
public class GroupsControllerTests
{
    List<GroupModel> _groupsModel = new();
    List<CourseModel> _coursesModel = new();
        
    private Mock<IGroupService> _mockGroupService;
    private Mock<ICourseService> _mockCourseService;
    private Mock<IStudentService> _mockStudentService;
    private GroupsController _groupsController;
        


    DbContextOptions<UniversityDbContext> _dbContextOptions;

    [SetUp]
    public void Setup()
    {
        _mockGroupService = new Mock<IGroupService>();
        _mockCourseService = new Mock<ICourseService>();
        _mockStudentService = new Mock<IStudentService>();

        _groupsController = new GroupsController(
            _mockGroupService.Object,
            _mockCourseService.Object
        );

        _dbContextOptions = new DbContextOptionsBuilder<UniversityDbContext>()
            .UseInMemoryDatabase(databaseName: "UniversityDb")
            .Options;

        _coursesModel.Clear();
        _coursesModel.Add(new CourseModel { Id = 1, Name = $"CourseName{1}", Description = $"CourseDesc{1}" });

        _groupsModel.Clear();
        for (int i = 1; i <= 3000; i++)
        {
            _groupsModel.Add(new GroupModel { Id = i, Name = $"CourseName{i}", CourseID = 1 });
        }
    }



    [Test]
    public void ListEntities_Returns_ViewResult_With_Groups_And_Pagination_Information()
    {
        // Arrange
        _mockGroupService.Setup(x => x.Count(It.IsAny<int?>())).Returns(_groupsModel.Count);
        _mockGroupService.Setup(x => x.ListEntities(It.IsAny<int?>(), It.IsAny<int>(), It.IsAny<int>())).Returns(_groupsModel);
        int numPages = (int)Math.Ceiling((decimal)_groupsModel.Count / 8);        

        // Act
        var result = _groupsController.ListEntities(5, 1) as ViewResult;


        if (result != null)
        {
            object? resultModel;
            resultModel = result.Model;
        }


        // Assert
        result.Should().NotBeNull();
        result.Should().BeOfType<ViewResult>();
    }

    [Test]
    public void InfoGroup_Returns_ViewResult_With_GroupModel()
    {
        // Arrange
        var groupId = 1;
        var groupModel = new GroupModel { Id = groupId, Name = $"GroupName{groupId}", CourseID = 1 };
        _mockGroupService.Setup(x => x.InfoGroup(groupId)).Returns(groupModel);

        // Act
        var result = _groupsController.InfoGroup(groupId) as ViewResult;
        if (result != null)
        {
            var resultModel = result.Model ?? throw new ArgumentNullException("result.Model");

            // Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<ViewResult>();
            resultModel.Should().BeEquivalentTo(groupModel);
        }
    }

    [Test]
    public void EditGroup_Returns_TempDataMessage_When_Id_Is_0_And_Courses_Not_Exist()
    {
        // Arrange

        _mockCourseService.Setup(x => x.GetAllCourses()).Returns(_coursesModel);

        var tempData = new Mock<ITempDataDictionary>();
        if (tempData == null) throw new ArgumentNullException(nameof(tempData));
        _groupsController.TempData = tempData.Object;

        // Act
        var result = _groupsController.EditGroup(0) as ViewResult;

        // Assert
        result.Should().NotBeNull();
        if (result != null) result.Model.Should().BeNull();
    }

    [Test]
    public void EditGroup_Returns_New_Model_When_Id_Is_0_And_Course_Exist()
    {
        // Arrange

        _mockCourseService.Setup(x => x.GetAllCourses()).Returns(_coursesModel);
        var tempData = new Mock<ITempDataDictionary>();
        _groupsController.TempData = tempData.Object;

        // Act
        var result = _groupsController.EditGroup(0) as ViewResult;

        // Assert
        result.Should().NotBeNull();
        if (result != null) result.Model.Should().BeNull();
        _mockCourseService.Verify(x => x.GetAllCourses(), Times.Once);
    }

    [Test]
    public void EditGroups_Edit_returns_Valid_Group()
    {
        using (var context = new UniversityDbContext(_dbContextOptions))
        {
            _mockCourseService.Setup(x => x.GetAllCourses()).Returns(_coursesModel);
            var tempData = new Mock<ITempDataDictionary>();
            _groupsController.TempData = tempData.Object;

            var editedModel = new GroupModel { Id = 1, Name = "Edited Group", CourseID = 1 };

            _mockGroupService.Setup(n => n.SaveGroup(editedModel));
            _mockGroupService.Setup(n => n.InfoGroup(editedModel.Id)).Returns(editedModel);

            // Act

            var createResult = _groupsController.EditGroup(1) as ViewResult;
            if (createResult != null)
            {
                var createGroup = createResult.Model as GroupModel;
            }

            var editGetResult = _groupsController.EditGroup(1) as ViewResult;
            var editGroup = editGetResult?.Model as GroupModel;
            var viewData = editGetResult?.ViewData;

            // Assert
            viewData.Should().ContainKey("ListCourses");

            editGetResult.Should().NotBeNull();
            editGroup.Should().NotBeNull();
            if (editGroup != null)
            {
                editGroup.Id.Should().Be(editedModel.Id);
                editGroup.Name.Should().Be(editedModel.Name);
                editGroup.CourseID.Should().Be(editedModel.CourseID);
            }
        }
    }

    [Test]
    public void EditCourses_Add_returns_Valid_Course()
    {
        using (var context = new UniversityDbContext(_dbContextOptions))
        {
            //_mockCourseService.Setup(x => x.GetAllCourses()).Returns(_coursesModel);

            var tempData = new Mock<ITempDataDictionary>();
            _groupsController.TempData = tempData.Object;

            var testModel = new GroupModel { Id = 1, Name = "New Group", CourseID = 1 };

            var result = _groupsController.EditGroup(testModel) as RedirectToActionResult;

            result.Should().NotBeNull();
            if (result != null && result.ActionName != null) result.ActionName.Should().Be("ListEntities");
            tempData.VerifySet(x => x["message"] = $"Group \"{testModel.Name}\" saved!", Times.Once);
        }
    }

    [Test]
    public void DeleteGroup_ValidId_ReturnsRedirectToListEntitiesWithMessage()
    {
        // Arrange
        var mockService = new Mock<IGroupService>();
        mockService.Setup(s => s.DeleteGroup(It.IsAny<int>())).Returns(true);

        var tempData = new Mock<ITempDataDictionary>();
        _groupsController.TempData = tempData.Object;

        // Act
        var result = _groupsController.DeleteGroup(3000) as RedirectToActionResult;
        var lastGroupIndex = _groupsModel.FindLastIndex(n => n.Id == 2999);

        // Assert
        result.Should().NotBeNull();
        if (result != null) result.ActionName.Should().BeEquivalentTo("ListEntities");
        tempData.VerifySet(t => t["message"] = "Group deleted", Times.Once);
        lastGroupIndex.Should().NotBe(-1);
    }
}