using System.Collections.Generic;
using University.Infrasructure.Models;

namespace University.Infrasructure.Services;

public interface IStudentService
{
    int Count(int? Id);
    IEnumerable<StudentModel> GetAllStudents();
    IEnumerable<StudentModel> ListEntities(int? parentGroupID);
    IEnumerable<StudentModel> ListEntities(int? ParentCourseId, int skip, int take);
    StudentModel InfoStudent(int Id);        
    void SaveStudent(StudentModel student);
    bool DeleteStudent(int Id);
}
