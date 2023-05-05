using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using University.Domain.Contracts;
using University.Domain.Entities;
using University.Domain.Presistent;


namespace University.Infrasructure.Repositories;

public class StudentRepository : IRepository <Student>
{
    private readonly UniversityDbContext _db;

    public StudentRepository(UniversityDbContext db)
    {
        _db = db ?? throw new ArgumentNullException(nameof(db));
    }
    public Student Add(Student entity)
    {
        _db.Students.Add(entity);
        _db.SaveChanges();
        return entity;
    }

    public bool DeleteById(int id)
    {
        Student? student = _db.Students.FirstOrDefault(x => x.Id== id);
        if (student!=null)
        {
            _db.Students.Remove(student);
            _db.SaveChanges();
            return true;
        }
        return false;
    }

    public Student FindById(int id)
    {
        return _db.Students.FirstOrDefault(x => x.Id== id);
    }

    public IEnumerable<Student> GetAll()
    {
        return _db.Students;
    }

    public IEnumerable<Student> GetFiltered(Expression<Func<Student, bool>> filter)
    {
        return _db.Students.Where(filter);
    }

    public IEnumerable<Student> GetPaged(int skip, int take)
    {
        return _db.Students.Skip(skip).Take(take);
    }

    public Student Update(Student entity)
    {
        Student? student = _db.Students.FirstOrDefault(x => x.Id == entity.Id);
        if (student!=null)
        {
            student.Id = entity.Id;
            student.FirstName = entity.FirstName;
            student.LastName = entity.LastName;
            student.GroupID = entity.GroupID;
            student.UpdatedAt = DateTime.UtcNow;
            return student;
        }
        return student;
    }

    public int Count()
    {
        return _db.Students.Count();
    }

    public int Count(Expression<Func<Student, bool>> filter)
    {
        return _db.Students.Count(filter);
    }

    public IEnumerable<Student> GetFilteredAndPaged(Expression<Func<Student, bool>> filter, int skip, int take)
    {
        return _db.Students.OrderBy(x => x.Id).Where(filter).Skip(skip).Take(take);
    }
}
