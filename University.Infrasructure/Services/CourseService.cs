using AutoMapper;
using System;
using System.Collections.Generic;
using University.Domain.Contracts;
using University.Infrasructure.Models;
using DomEntities = University.Domain.Entities;

namespace University.Infrasructure.Services;

public class CourseService : ICourseService
{
    private IRepository<DomEntities.Course> _repoCourse;
    private IMapper _mapper;
    private CourseModel? _modelCourse;

    public CourseService(IRepository<DomEntities.Course> repoCourse, IMapper mapper)
    {
        this._repoCourse = repoCourse ?? throw new ArgumentNullException(nameof(repoCourse));
        this._mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));            
    }


    public int Count()
    {
        return _repoCourse.Count();
    }
   

    public IEnumerable<CourseModel> GetAllCourses()
    {
        var courses = _repoCourse.GetAll();
        return _mapper.Map<IEnumerable<CourseModel>>(courses); 
    }


    public CourseModel AddCourse()
    {            
        return _modelCourse;
    }
    

    public IEnumerable<CourseModel> ListEntities(int skip, int take)
    {
        var courses = _repoCourse.GetPaged(take, skip);
        return _mapper.Map<IEnumerable<CourseModel>>(courses);
    }


    public bool DeleteCourse(int Id)
    {
        bool isDeleted = _repoCourse.DeleteById(Id);
        return isDeleted;
    }


    public void SaveCourse(CourseModel course)
    {            
        if (course.Id != 0)
        {                
            _repoCourse.Update(_mapper.Map<DomEntities.Course>(course));
        }
        else
        {
            _repoCourse.Add(_mapper.Map<DomEntities.Course>(course));
        }

    }

    public CourseModel InfoCourse(int Id)
    {
        var course = _repoCourse.FindById(Id);

        return _mapper.Map<CourseModel>(course);
    }
}
