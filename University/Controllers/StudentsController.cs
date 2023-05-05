using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Diagnostics;
using University.Infrasructure.Models;
using University.Infrasructure.Services;
using University.Models;

namespace Students_MVC.Controllers;

public class StudentsController : Controller
{
    private const int STUDENTS_ON_PAGE = 8;
    private IStudentService _studentService;
    private IGroupService _groupService;

    public StudentsController(IStudentService studentService, IGroupService groupService)
    {
        _studentService = studentService ?? throw new ArgumentNullException(nameof(studentService));
        _groupService = groupService ?? throw new ArgumentNullException(nameof(groupService));
    }  

    public IActionResult ListEntities(int? parentGroupId, int? page)
    {
        int numPages = (int)Math.Ceiling((decimal)_studentService.Count(parentGroupId) / STUDENTS_ON_PAGE);
        int currentPage = page ?? 1;
        int skip = (currentPage - 1) * STUDENTS_ON_PAGE;
        int take = STUDENTS_ON_PAGE;        

        var listStudents = _studentService.ListEntities(parentGroupId, skip, take);
        var model = (listStudents, numPages, currentPage);

        return View(model);
    }

    public IActionResult InfoStudent(int Id)
    {
        return View(_studentService.InfoStudent(Id));
    }


    [HttpGet]
    public IActionResult EditStudent(int? Id)
    {
        var groups = _groupService.GetAllGroups().ToList();
        int groupsCount = groups.Count;
        ViewBag.ListGroups = new SelectList(groups, "Id", "Name" );
        if (Id != null & Id != 0)
            return View(_studentService.InfoStudent(Id.Value));

        if (groupsCount > 0)
            return View();

        TempData["message"] = $"you can't create a student while no groups exist. Create a group first";
        return RedirectToAction("ListEntities");
    }

    [HttpPost]
    public IActionResult EditStudent(StudentModel student)
    {
        _studentService.SaveStudent(student);

        TempData["message"] = $"Student \"{student.FirstName}, {student.LastName} \" saved!";

        return RedirectToAction("ListEntities");
    }


    public IActionResult DeleteStudent(int Id)
    {
        _studentService.DeleteStudent(Id);
        TempData["message"] = $"Student deleted";

        return RedirectToAction("ListEntities");
    }


    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
