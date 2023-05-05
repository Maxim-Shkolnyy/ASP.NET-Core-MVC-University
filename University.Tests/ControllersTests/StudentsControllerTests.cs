using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.EntityFrameworkCore;
using Moq;
using University.Domain.Presistent;
using University.Infrasructure.Models;
using University.Infrasructure.Services;
using Students_MVC.Controllers;

namespace University.Tests.Controllers;

[TestFixture]
public class StudentsControllerTests
{
    List<GroupModel> _groupsModel = new();
    List<CourseModel> _coursesModel = new();
    List<StudentModel> _studentsModel = new();
    private Mock<IGroupService> _mockGroupService;
    private Mock<ICourseService> _mockCourseService;
    private Mock<IStudentService> _mockStudentService;

    [SetUp]
    public void Setup()
    {
        _mockGroupService = new Mock<IGroupService>();
        _mockCourseService = new Mock<ICourseService>();
        _mockStudentService = new Mock<IStudentService>();

        _coursesModel.Clear();
        _coursesModel.Add(new CourseModel { Id = 1, Name = $"CourseName{1}", Description = $"CourseDesc{1}" });

        _groupsModel.Clear();
        _groupsModel.Add(new GroupModel { Id = 1, Name = $"GroupName{1}", CourseID = 1 });

        _studentsModel.Clear();
        for (int i = 1; i <= 3000; i++)
        {
            _studentsModel.Add(new StudentModel { Id = i, FirstName = $"FirstName{i}", LastName = $"LastName{i}", GroupID = 1 });
        }
    }


    [Test]
    public void ListEntities_ReturnsViewResult()
    {
        var mockStudentService = new Mock<IStudentService>();
        mockStudentService.Setup(m => m.ListEntities(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>()))
            .Returns(_studentsModel);

        var studentController = new StudentsController(_mockStudentService.Object, _mockGroupService.Object);
        
        // Act
        IActionResult result = studentController.ListEntities(1, 5);

        // Assert
        result.Should().BeOfType<ViewResult>();
    }   


    [Test]
    public void ListEntities_ReturnsCorrectPages()
    {
        var addedModel = new StudentModel { Id = 3001, FirstName = "FirstName3001", LastName = "LastName3001", GroupID = 1 };
        _mockStudentService.Setup(m => m.ListEntities(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>())).Returns(_studentsModel);
        _mockStudentService.Setup(m => m.Count(1)).Returns(_studentsModel.Count());
        _mockStudentService.Setup(m => m.InfoStudent(1)).Returns(addedModel);

        var studentController = new StudentsController(_mockStudentService.Object, _mockGroupService.Object);

        // Arrange
        int? page = 1;

        // Act
        var result = studentController.ListEntities(1, 1) as ViewResult;
        if (result != null)
        {
            var model = result.Model as (IEnumerable<StudentModel>, int, int)?;

            int corectPages = (int)Math.Ceiling(_studentsModel.Count() / 8.0);

            // Assert
            if (model != null)
            {
                model.Value.Item1.Should().NotBeNullOrEmpty();
                model.Value.Item2.Should().Be(corectPages);
                model.Value.Item3.Should().Be(page);

            }
        }
    }

    [Test]
    public void EditStudent_Get_UpdateAndAddValidCourse()
    {
        // Arrange     
        var studentController = new StudentsController(_mockStudentService.Object, _mockGroupService.Object);
        _mockGroupService.Setup(n => n.GetAllGroups()).Returns(_groupsModel);

        var editedModel = new StudentModel { Id = 1, FirstName = "FirstName1", LastName = "LastName1", GroupID = 1 };

        var mockService = new Mock<IStudentService>();
        mockService.Setup(n => n.InfoStudent(editedModel.Id)).Returns(editedModel);

        // Act
        var result = studentController.EditStudent(editedModel.Id) as ViewResult;
        

        // Assert
        _mockGroupService.Verify(n => n.GetAllGroups(), Times.Once);
        _mockStudentService.Verify(n => n.InfoStudent(editedModel.Id), Times.Once);
    }

    [Test]
    public void EditStudent_Post_ReturnsRedirectToAction()
    {
        // Arrange
        var studentId = 1;
        var mockStudentService = new Mock<IStudentService>();
        var controller = new StudentsController(_mockStudentService.Object, _mockGroupService.Object);
        var tempData = new Mock<ITempDataDictionary>();
        controller.TempData = tempData.Object;
        var StudentModel = new StudentModel { Id = studentId, FirstName = "New FirstName", LastName = "New LastName", GroupID = 1 };
        mockStudentService.Setup(x => x.SaveStudent(StudentModel));

        // Act
        var result = controller.EditStudent(StudentModel) as RedirectToActionResult;

        // Assert
        result.Should().NotBeNull().And.BeOfType<RedirectToActionResult>();
        if (result != null)
        {
            result.ActionName.Should().Be("ListEntities");
            result.RouteValues.Should().BeNull();
        }

        tempData.VerifySet(x => x["message"] = $"Student \"{StudentModel.FirstName}, {StudentModel.LastName} \" saved!", Times.Once);
    }




    [Test]
    public void DeleteStudent_ValidId_ReturnsRedirectToListEntitiesWithMessage()
    {
        // Arrange
        var mockService = new Mock<IStudentService>();
        mockService.Setup(s => s.DeleteStudent(It.IsAny<int>())).Returns(true);

        var studentController = new StudentsController(_mockStudentService.Object, _mockGroupService.Object);

        var tempData = new Mock<ITempDataDictionary>();
        studentController.TempData = tempData.Object;

        // Act
        var result = studentController.DeleteStudent(3000) as RedirectToActionResult;

        // Assert
        result.Should().NotBeNull();
        if (result != null) result.ActionName.Should().BeEquivalentTo("ListEntities");
        tempData.VerifySet(t => t["message"] = $"Student deleted", Times.Once);

    }
}