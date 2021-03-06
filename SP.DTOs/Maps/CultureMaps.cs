using SP.DataAccess;
using SP.Dto;
using System;
using System.Linq.Expressions;
namespace SP.Dto.Maps
{
    internal static class CultureMaps
    {
        internal static Func<CultureDto, Culture> mapToRepo()
        {
            return m => new Culture
            {
                LocaleCode = m.LocaleCode,
                Name = m.Name,
                CountryCode = m.CountryCode
                //Hospitals = m.Hospitals,
                //ProfessionalRoles = m.ProfessionalRoles
            };
        }

        internal static Expression<Func<Culture, CultureDto>> mapFromRepo()
        {
            return m => new CultureDto {
            LocaleCode = m.LocaleCode,
            Name = m.Name,
            CountryCode = m.CountryCode
            //Hospitals = m.Hospitals,
            //ProfessionalRoles = m.ProfessionalRoles
        };
    }
    }
}
