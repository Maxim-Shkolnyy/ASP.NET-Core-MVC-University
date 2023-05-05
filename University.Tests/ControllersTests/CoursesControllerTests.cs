using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Moq;
using University.Infrasructure.Controllers;
using University.Infrasructure.Models;
using University.Infrasructure.Services;

namespace University.Tests.Controllers;

[TestFixture]
public class CoursesControllerTests
{
    List<CourseModel> _coursesModel = new();

    [SetUp]
    public void Setup()
    {   
        _coursesModel.Clear();
        for (int i = 1; i <= 3000; i++)
        {
            _coursesModel.Add(new CourseModel { Id = i, Name = $"CourseName{i}", Description = $"CourseDesc{i}" });
        }
    }


    [Test]
    public void ListEntities_ReturnsViewResult()
    {
        var mockCourseService = new Mock<ICourseService>();
        mockCourseService.Setup(m => m.ListEntities(It.IsAny<int>(), It.IsAny<int>()))
            .Returns(_coursesModel);

        var courseController = new CoursesController(mockCourseService.Object, new CourseModel());

        // Arrange
        int? page = 1;

        // Act
        IActionResult result = courseController.ListEntities(page);

        // Assert
        result.Should().BeOfType<ViewResult>();
    }


    [Test]
    public void ListEntities_ReturnsCorrectViewData()
    {
        var mockService = new Mock<ICourseService>();
        mockService.Setup(m => m.ListEntities(It.IsAny<int>(), It.IsAny<int>()))
            .Returns(_coursesModel);
        mockService.Setup(m => m.Count()).Returns(_coursesModel.Count());

        var courseController = new CoursesController(mockService.Object, new CourseModel());

        // Arrange
        int page = 1;

        // Act
        var result = courseController.ListEntities(page) as ViewResult;
        if (result != null)
        {
            var model = result.Model as (IEnumerable<CourseModel>, int, int)?;

            // Assert
            if (model != null)
            {
                model.Value.Item1.Should().NotBeNullOrEmpty();
                model.Value.Item2.Should().BeGreaterThan(0);
                model.Value.Item3.Should().Be(page);
            }
        }
    }


    [Test]
    public void ListEntities_ReturnsCorrectPages()
    {
        var mockService = new Mock<ICourseService>();
        mockService.Setup(m => m.ListEntities(It.IsAny<int>(), It.IsAny<int>()))
            .Returns(_coursesModel);
        mockService.Setup(m => m.Count()).Returns(_coursesModel.Count());

        var courseController = new CoursesController(mockService.Object, new CourseModel());

        // Arrange
        int? page = 1;

        // Act
        var result = courseController.ListEntities(page) as ViewResult;
        if (result != null)
        {
            var model = result.Model as (IEnumerable<CourseModel>, int, int)?;

            int corectPages = (int)Math.Ceiling(_coursesModel.Count() / 8.0);

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
    public void EditCourses_Get_UpdateAndAddValidCourse()
    {        
        // Arrange
        var mockService = new Mock<ICourseService>();
        var courseController = new CoursesController(mockService.Object, new CourseModel());

        var addedModel = new CourseModel { Id = 3001, Name = "New Course", Description = "New Course Description" };
        var editedModel = new CourseModel { Id = 1, Name = "Edited Course", Description = "Edited Course Description" };

        mockService.Setup(n => n.AddCourse()).Returns(addedModel);
        mockService.Setup(n => n.InfoCourse(editedModel.Id)).Returns(editedModel);
        mockService.Setup(n => n.SaveCourse(editedModel));

        // Act
        var createResult = courseController.EditCourse(0) as ViewResult;
        if (createResult != null)
        {
            CourseModel? createCourse = createResult.Model as CourseModel;
            var editResult = courseController.EditCourse(1) as ViewResult;

            if (editResult != null)
            {
                CourseModel? editCourse = editResult.Model as CourseModel;

                // Assert
                createResult.Should().NotBeNull();
                createCourse.Should().NotBeNull();
                if (createCourse != null)
                {
                    createCourse.Id.Should().Be(addedModel.Id);
                    createCourse.Name.Should().Be(addedModel.Name);
                    createCourse.Description.Should().Be(addedModel.Description);
                }

                editResult.Should().NotBeNull();
                editCourse.Should().NotBeNull();
                if (editCourse != null)
                {
                    editCourse.Id.Should().Be(editedModel.Id);
                    editCourse.Name.Should().Be(editedModel.Name);
                    editCourse.Description.Should().Be(editedModel.Description);
                }
            }
        }        
    }

    [Test]
    public void EditCourse_Post_ReturnsRedirectToAction()
    {
        
        // Arrange
        var courseId = 1;
        var mockCourseService = new Mock<ICourseService>();
        var controller = new CoursesController(mockCourseService.Object, new CourseModel());
        var tempData = new Mock<ITempDataDictionary>();
        controller.TempData = tempData.Object;
        var courseModel = new CourseModel { Id = courseId, Name = "Test Course", Description = "New Course Description" };       
        mockCourseService.Setup(x => x.SaveCourse(courseModel));        

        // Act
        var result = controller.EditCourse(courseModel) as RedirectToActionResult;

        // Assert
        mockCourseService.Verify(service => service.SaveCourse(courseModel), Times.Once);
        result.Should().NotBeNull().And.BeOfType<RedirectToActionResult>();
        if (result != null)
        {
            result.ActionName.Should().Be("ListEntities");
            result.RouteValues.Should().BeNull();
        }

        tempData.VerifySet(x => x["message"] = $"Course \"{courseModel.Name}\" saved!", Times.Once);
    }

    [Test]
    public void DeleteCourse_ValidId_ReturnsRedirectToListEntitiesWithMessage()
    {
        // Arrange
        var mockService = new Mock<ICourseService>();
        mockService.Setup(s => s.DeleteCourse(It.IsAny<int>())).Returns(true);

        var courseController = new CoursesController(mockService.Object, new CourseModel());

        var tempData = new Mock<ITempDataDictionary>();
        courseController.TempData = tempData.Object;

        // Act
        var result = courseController.DeleteCourse(3000) as RedirectToActionResult;

        // Assert
        result.Should().NotBeNull();
        if (result != null) result.ActionName.Should().BeEquivalentTo("ListEntities");
        tempData.VerifySet(t => t["message"] = "Course deleted", Times.Once);

    }
}