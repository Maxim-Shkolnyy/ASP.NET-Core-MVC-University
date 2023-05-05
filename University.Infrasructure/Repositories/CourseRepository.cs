using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using University.Domain.Contracts;
using University.Domain.Entities;
using University.Domain.Presistent;

namespace University.Infrasructure.Repositories;

public class CourseRepository : IRepository <Course>
{
    private readonly UniversityDbContext _db;

    public CourseRepository(UniversityDbContext db)
    {
        this._db = db?? throw new ArgumentNullException(nameof(db)) ;
    }

    public Course Add(Course entity)
    {
       _db.Courses.Add(entity);
        _db.SaveChanges();
        return entity;
    }

    public bool DeleteById(int id)
    {
        var course = _db.Courses.FirstOrDefault(x => x.Id == id);

        if (course?.Id == null || _db.Groups.Count(x => x.CourseID == id) > 0)
            return false;

        _db.Courses.Remove(course);
        _db.SaveChanges();
        return true;
    }


    public Course FindById(int id)
    {
        return _db.Courses.FirstOrDefault(x => x.Id == id);
    }

    public IEnumerable<Course> GetAll()
    {
        return _db.Courses;
    }

    public IEnumerable<Course> GetFiltered(Expression<Func<Course, bool>> filter)
    {
        return _db.Courses.Where(filter);           
    }

    public IEnumerable<Course> GetPaged(int take, int skip)
    {
        return _db.Courses.OrderBy(x=>x.Id).Skip(skip).Take(take);
    }

    public Course Update(Course entity)
    {
        Course? course = _db.Courses.FirstOrDefault(x => x.Id == entity.Id);
        
        if (course != null)
        {
            course.Id = entity.Id;
            course.Name = entity.Name;
            course.Description = entity.Description;
            course.UpdatedAt = DateTime.UtcNow;
            _db.SaveChanges();
        }
        return course;
    }

    public int Count()
    {
        return _db.Courses.Count();
    }

    public int Count(Expression<Func<Course, bool>> filter)
    {
        return _db.Courses.Count(filter);
    }

    public IEnumerable<Course> GetFilteredAndPaged(Expression<Func<Course, bool>> filter, int take, int skip)
    {
        return _db.Courses.OrderBy(x => x.Id).Where(filter).Skip(skip).Take(take);
    }
}
