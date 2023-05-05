using University.Domain.Entities;
using University.Infrasructure.Models;

namespace University.Infrasructure.Mappers;

public class GroupProfile : MappingProfile
{
    public GroupProfile()
    {
        CreateMap<Group, GroupModel>();
        CreateMap<GroupModel, Group>();
    }
}
