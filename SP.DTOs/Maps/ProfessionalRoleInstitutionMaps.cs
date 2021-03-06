using SP.DataAccess;
using System;
using System.Linq.Expressions;
namespace SP.Dto.Maps
{
    internal static class ProfessionalRoleInstitutionMaps
    {
        internal static Func<ProfessionalRoleInstitutionDto, ProfessionalRoleInstitution> mapToRepo()
        {
            return m => new ProfessionalRoleInstitution
            {
                InstitutionId = m.InstitutionId,
                ProfessionalRoleId = m.ProfessionalRoleId
            };
        }


        internal static Expression<Func<ProfessionalRoleInstitution, ProfessionalRoleInstitutionDto>> mapFromRepo()
        {
            return m => new ProfessionalRoleInstitutionDto
            {
                InstitutionId = m.InstitutionId,
                ProfessionalRoleId = m.ProfessionalRoleId
            };
        }
    }
}
