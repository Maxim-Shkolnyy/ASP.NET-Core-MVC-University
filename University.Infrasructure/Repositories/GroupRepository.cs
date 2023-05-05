using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using University.Domain.Contracts;
using University.Domain.Entities;
using University.Domain.Presistent;


namespace University.Infrasructure.Repositories;

public class GroupRepository : IRepository<Group>
{
    private readonly UniversityDbContext _db;

    public GroupRepository(UniversityDbContext db)
    {
        _db = db ?? throw new ArgumentNullException(nameof(db));
    }
    public Group Add(Group entity)
    {
        _db.Groups.Add(entity);
        _db.SaveChanges();
        return entity;
    }

    public bool DeleteById(int id)
    {
        Group? group = _db.Groups.FirstOrDefault(x => x.Id == id);
        int? studentCount = _db.Students.Count(x => x.GroupID == id);
        if (group == null || studentCount > 0)
        {
            return false;
        }
        _db.Groups.Remove(group);
        _db.SaveChanges();
        return true;
    }

    public Group FindById(int id)
    {
        //Group group = _db.Groups.FirstOrDefault(x => x.Id == id);
        //if (id != null && id != 0)
        //    return group;

        //group.Id = 0;
        //group.Name = "_";
        //group.CourseID = 0;
        //group.CreatedAt = DateTime.Now;
        //return group;

         return _db.Groups.FirstOrDefault(x => x.Id == id);            
    }

    public IEnumerable<Group> GetAll()
    {
        return _db.Groups;
    }

    public IEnumerable<Group> GetFiltered(Expression<Func<Group, bool>> filter)
    {
        return _db.Groups.Where(filter);
    }

    public IEnumerable<Group> GetPaged(int skip, int take)
    {
        return _db.Groups.OrderBy(x => x.Id).Skip(skip).Take(take);
    }



    public Group Update(Group entity)
    {
        Group? group = _db.Groups.FirstOrDefault(x => x.Id == entity.Id);
        if (group != null)
        {
            group.Id = entity.Id;
            group.Name = entity.Name;
            group.CourseID = entity.CourseID;
            group.UpdatedAt = DateTime.UtcNow;
            _db.SaveChanges();
            return group;
        }
        return group;
    }

    public int Count()
    {
        return _db.Groups.Count();
    }

    public int Count(Expression<Func<Group, bool>> filter)
    {
        return _db.Groups.Count(filter);
    }

    public IEnumerable<Group> GetFilteredAndPaged(Expression<Func<Group, bool>> filter, int take, int skip)
    {
        return _db.Groups.OrderBy(x => x.Id).Where(filter).Skip(skip).Take(take);
    }
}
