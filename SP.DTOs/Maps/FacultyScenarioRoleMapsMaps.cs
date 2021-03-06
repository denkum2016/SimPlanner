using SP.DataAccess;
using SP.Dto;
using System;
using System.Linq.Expressions;
namespace SP.Dto.Maps
{
    internal static class FacultyScenarioRoleMaps
    {
        internal static Func<FacultyScenarioRoleDto, FacultyScenarioRole> mapToRepo()
        {
            return m => new FacultyScenarioRole
            {
                Id = m.Id,
                Description = m.Description,
                Order = m.Order
                //CourseTypes = m.CourseTypes,
                //ScenarioFacultyRoles = m.ScenarioFacultyRoles
            };
        }

        internal static Expression<Func<FacultyScenarioRole, FacultyScenarioRoleDto>> mapFromRepo()
        {
            return m => new FacultyScenarioRoleDto
            {
                Id = m.Id,
                Description = m.Description,
                Order = m.Order
                //CourseTypes = m.CourseTypes,
                //ScenarioFacultyRoles = m.ScenarioFacultyRoles
            };
        }
    }
}
