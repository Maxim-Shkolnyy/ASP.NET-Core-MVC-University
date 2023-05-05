using University.Domain.Entities;
using University.Infrasructure.Models;

namespace University.Infrasructure.Mappers;

public class StudentProfile : MappingProfile
{
    public StudentProfile() 
    {
        CreateMap<Student, StudentModel>();
        CreateMap<StudentModel, Student>();
    }
}
