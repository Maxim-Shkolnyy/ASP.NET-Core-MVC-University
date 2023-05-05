using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using University.Domain.Contracts;
using DomEntities = University.Domain.Entities;
using University.Infrasructure.Models;


namespace University.Infrasructure.Services;

public class GroupService : IGroupService
{
    private IRepository<DomEntities.Group> _repoGroup;
    private readonly IMapper _mapper;


    public GroupService(IRepository<DomEntities.Group> repoGroup, IMapper mapper)
    {
        _repoGroup = repoGroup ?? throw new ArgumentNullException(nameof(repoGroup));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    }

    public IEnumerable<GroupModel> GetAllGroups()
    {
        var courses = _repoGroup.GetAll();
        return _mapper.Map<IEnumerable<GroupModel>>(courses);
    }

    public int Count(int? ParentCourseId)
    {
        if (ParentCourseId == null)
            return _repoGroup.Count();

        return _repoGroup.Count(x => x.CourseID == ParentCourseId);
    }

    public IEnumerable<GroupModel> ListEntities(int? ParentCourseId)
    {
        var groups = _repoGroup.GetAll().Where(n => n.CourseID == ParentCourseId);
        return _mapper.Map<IEnumerable<GroupModel>>(groups);
    }

    public IEnumerable<GroupModel> ListEntities(int skip, int take)
    {
        var groups = _repoGroup.GetPaged(skip, take);
        return _mapper.Map<IEnumerable<GroupModel>>(groups);
    }

    public IEnumerable<GroupModel> ListEntities(int? ParentCourseId, int skip, int take)
    {
        var groupsPaged = _repoGroup.GetPaged(skip, take);

        if (ParentCourseId == null)
            return _mapper.Map<IEnumerable<GroupModel>>(groupsPaged);

        var groupsFilteredAndPaged = _repoGroup.GetFilteredAndPaged(x => x.CourseID == ParentCourseId, take, skip);
        return _mapper.Map<IEnumerable<GroupModel>>(groupsFilteredAndPaged);
    }


    public GroupModel InfoGroup(int Id)
    {
        var group = _repoGroup.FindById(Id);
        return _mapper.Map<GroupModel>(group);
    }

    public void SaveGroup(GroupModel group)
    {

        var findedGroup = _repoGroup.FindById(group.Id);
        if (group != null && findedGroup != null)
        {
            _repoGroup.Update(_mapper.Map<DomEntities.Group>(group));
        }
        else
        {
            _repoGroup.Add(_mapper.Map<DomEntities.Group>(group));
        }
    }

    public bool DeleteGroup(int Id)
    {
        bool isDeleted = _repoGroup.DeleteById(Id);
        return isDeleted;
    }
}