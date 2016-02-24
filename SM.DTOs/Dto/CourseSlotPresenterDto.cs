using SM.DataAccess.Metadata;
using System;
using System.ComponentModel.DataAnnotations;
namespace SM.Dto
{
    [MetadataType(typeof(CourseSlotPresenterMetadata))]
    public class CourseSlotPresenterDto
    {
        public Guid CourseId { get; set; }
        public Guid CourseSlotId { get; set; }
        public Guid PresenterId { get; set; }
        public CourseDto Course { get; set; }
        public CourseSlotDto CourseSlot { get; set; }
        public ParticipantDto Presenter { get; set; }

    }
}