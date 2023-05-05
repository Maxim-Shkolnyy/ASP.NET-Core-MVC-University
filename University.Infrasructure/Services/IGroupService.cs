using System.Collections.Generic;
using University.Infrasructure.Models;

namespace University.Infrasructure.Services;

public interface IGroupService
{
    IEnumerable<GroupModel> GetAllGroups();
    int Count(int? ParentCourseId);
    IEnumerable<GroupModel> ListEntities(int? ParentCourseId);
    IEnumerable<GroupModel> ListEntities(int skip, int take);
    IEnumerable<GroupModel> ListEntities(int? ParentCourseId, int skip, int take);
    GroupModel InfoGroup(int Id);        
    void SaveGroup(GroupModel group);
    bool DeleteGroup(int Id);        
}
