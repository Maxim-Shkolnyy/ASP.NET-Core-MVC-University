using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Diagnostics;
using University.Infrasructure.Models;
using University.Infrasructure.Services;
using University.Models;

namespace MVC.Controllers;

public class GroupsController : Controller
{
    private const int GROUPS_ON_PAGE = 8;

    private IGroupService _groupService;
    private ICourseService _courseService;
    

    public GroupsController(IGroupService groupService, ICourseService courseService)
    {
        _groupService = groupService ?? throw new ArgumentNullException(nameof(groupService));
        _courseService = courseService ?? throw new ArgumentNullException(nameof(courseService));
    }

    public IActionResult ListEntities(int? Id, int? page)
    {
        int numPages = (int)Math.Ceiling((decimal)_groupService.Count(Id) / GROUPS_ON_PAGE);
        int currentPage = page ?? 1;
        int skip = (currentPage - 1) * GROUPS_ON_PAGE;
        int take = GROUPS_ON_PAGE;

        var listGroups = _groupService.ListEntities(Id, skip, take);
        var model = (listGroups, numPages, currentPage);

        return View(model);
    }    

    public IActionResult InfoGroup(int Id)
    {
        return View(_groupService.InfoGroup(Id));
    }

    [HttpGet]
    public IActionResult EditGroup(int? Id)
    {
        var courses = _courseService.GetAllCourses().ToList();
        int coursesCount = courses.Count;
        ViewBag.ListCourses = new SelectList(courses, "Id", "Name");
        if (Id != null & Id != 0)        
            return View(_groupService.InfoGroup(Id.Value));
        
            if(coursesCount > 0)
            return View();

        TempData["message"] = $"you can't create a group while no courses exist. Create a course first";
        return RedirectToAction("ListEntities");
    }

    [HttpPost]
    public IActionResult EditGroup(GroupModel group)
    {
        _groupService.SaveGroup(group);

        TempData["message"] = $"Group \"{group.Name}\" saved!";

        return RedirectToAction("ListEntities");
    }


    public IActionResult DeleteGroup(int Id)
    {
        bool isDeleted= _groupService.DeleteGroup(Id);
        if (isDeleted)
        {
            TempData["message"] = $"Group not deleted because it is not empty!";
        }
        
        TempData["message"] = $"Group deleted";

        return RedirectToAction("ListEntities");
    }


    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
