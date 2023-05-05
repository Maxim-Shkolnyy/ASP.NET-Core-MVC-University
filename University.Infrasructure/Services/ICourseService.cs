using System.Collections.Generic;
using University.Infrasructure.Models;

namespace University.Infrasructure.Services;

public interface ICourseService
{
    IEnumerable<CourseModel> GetAllCourses();
    CourseModel AddCourse();
    int Count();
    IEnumerable<CourseModel> ListEntities(int skip, int take);
    CourseModel InfoCourse(int Id);
    void SaveCourse(CourseModel course);
    bool DeleteCourse(int Id);        

}
