using University.Domain.Entities;
using University.Infrasructure.Models;

namespace University.Infrasructure.Mappers;

public class CourseProfile : MappingProfile
{
    public CourseProfile() 
    {
        CreateMap<Course,CourseModel> ();
        CreateMap<CourseModel, Course> ();
    }
}
