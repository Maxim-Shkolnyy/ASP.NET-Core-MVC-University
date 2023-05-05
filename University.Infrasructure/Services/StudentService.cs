using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using University.Domain.Contracts;
using University.Infrasructure.Models;
using DomEntities = University.Domain.Entities;

namespace University.Infrasructure.Services;

public class StudentService : IStudentService
{
    private readonly IRepository<DomEntities.Student> _repoStudent;
    private readonly IMapper _mapper;

    public StudentService(IRepository<DomEntities.Student> repoStudent, IMapper mapper)
    {
        _repoStudent = repoStudent ?? throw new ArgumentNullException(nameof(repoStudent));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    }

    public int Count(int? Id)
    {
        if (Id == null)
            return _repoStudent.Count();

        return _repoStudent.Count(x => x.GroupID == Id);
    }

    public IEnumerable<StudentModel> GetAllStudents()
    {
        var students = _repoStudent.GetAll();
        return _mapper.Map<IEnumerable<StudentModel>>(students);
    }

    public bool DeleteStudent(int Id)
    {        
        return _repoStudent.DeleteById(Id);
    }

    public IEnumerable<StudentModel> ListEntities(int? ParentCourseId, int skip, int take)
    {
        var studentsPaged = _repoStudent.GetPaged(skip, take);

        if (ParentCourseId == null)        
            return _mapper.Map<IEnumerable<StudentModel>>(studentsPaged);

        var studentsFilteredAndPaged = _repoStudent.GetFilteredAndPaged(x => x.GroupID == ParentCourseId, skip, take);
        return _mapper.Map<IEnumerable<StudentModel>>(studentsFilteredAndPaged);       
    }


    public IEnumerable<StudentModel> ListEntities(int? parentGroupID)
    {
        var students = _repoStudent.GetAll().Where(x => x.GroupID == parentGroupID);

        return _mapper.Map<IEnumerable<StudentModel>>(students);
    }

    StudentModel IStudentService.InfoStudent(int Id)
    {
        var student = _repoStudent.FindById(Id);
        return _mapper.Map<StudentModel>(student);
    }

    public void SaveStudent(StudentModel student)
    {
        if (student.Id != 0)
        {
            _repoStudent.Update(_mapper.Map<DomEntities.Student>(student));
        }
        else
        {
            _repoStudent.Add(_mapper.Map<DomEntities.Student>(student));
        }
    }        
}

