using SP.DataAccess;
using SP.Dto;
using System;
using System.Linq.Expressions;
namespace SP.Dto.Maps
{
    internal static class CourseFormatMaps
    {
        internal static Func<CourseFormatDto, CourseFormat> mapToRepo()
        {
            return m => new CourseFormat
            {
                Id = m.Id,
                DaysDuration = m.DaysDuration,
                Description = m.Description,
                CourseTypeId = m.CourseTypeId
                
            };
        }

        internal static Expression<Func<CourseFormat, CourseFormatDto>> mapFromRepo()
        {
            return m => new CourseFormatDto
            {
                Id = m.Id,
                DaysDuration = m.DaysDuration,
                Description = m.Description,
                CourseTypeId = m.CourseTypeId
            };
        }
    }
}
