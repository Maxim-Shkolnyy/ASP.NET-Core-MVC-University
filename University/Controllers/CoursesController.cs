using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using University.Models;
using University.Infrasructure.Services;
using University.Infrasructure.Models;
using University.MVC.Models;

namespace University.Infrasructure.Controllers;

public class CoursesController : Controller
{

    private const int COURSES_ON_PAGE = 8;
    private readonly ICourseService _courseService;

    private readonly CourseModel _courseModel;
    private readonly CardViewModel _viewModels;
    

    public CoursesController(ICourseService courseService, CourseModel courseModel)
    {
        _courseService = courseService ?? throw new ArgumentNullException(nameof(courseService));
        _courseModel = courseModel ?? throw new ArgumentNullException(nameof(courseModel));
        _viewModels = new CardViewModel();
    }

    public IActionResult ListEntities(int? page)
    {
        int numPages = (int)Math.Ceiling((decimal)_courseService.Count() / COURSES_ON_PAGE);
        int currentPage = page ?? 1;
        int skip = (currentPage - 1) * COURSES_ON_PAGE;
        int take = COURSES_ON_PAGE;

        var listCourses = _courseService.ListEntities(skip, take);
        var model = (listCourses, numPages, currentPage);

        return View(model);
    }

    public IActionResult InfoCourse(int Id)
    {
        return View(_courseService.InfoCourse(Id));

    }


    [HttpGet]
    public IActionResult EditCourse(int? Id)
    {
        if (Id != null & Id != 0)
        {
            return View(_courseService.InfoCourse(Id.Value));
        }
                    
        return View(_courseService.AddCourse());
    }

    [HttpPost]
    public IActionResult EditCourse(CourseModel course)
    {
        _courseService.SaveCourse(course);

        TempData["message"] = $"Course \"{course.Name}\" saved!";

        return RedirectToAction("ListEntities");
    }


    public IActionResult DeleteCourse(int Id)
    {
        bool isDeleted = _courseService.DeleteCourse(Id);
        if (isDeleted)
        {
            TempData["message"] = $"Course deleted";
        }
        else
        {
            TempData["message"] = $"Course not deleted because it is not empty!";
        }

        return RedirectToAction("ListEntities");
    }


    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}