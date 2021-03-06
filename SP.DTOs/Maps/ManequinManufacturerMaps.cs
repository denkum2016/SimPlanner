using SP.DataAccess;
using SP.Dto;
using System;
using System.Linq.Expressions;
namespace SP.Dto.Maps
{
    internal static class ManequinManufacturerMaps
    {
        internal static Func<ManequinManufacturerDto, ManequinManufacturer> mapToRepo()
        {
            return m => new ManequinManufacturer
            {
                Id = m.Id,
                Name = m.Name
            };
        }

        internal static Expression<Func<ManequinManufacturer, ManequinManufacturerDto>> mapFromRepo()
        {
            return m => new ManequinManufacturerDto
            {
                Id = m.Id,
                Name = m.Name
            };
        }
    }
}
