using SP.Metadata;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SP.Dto
{
    [MetadataType(typeof(CourseMetadata))]
    public class CourseDto
    {
        public Guid Id { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime FinishTime { get; set; }
        public DateTime? FacultyMeetingTime { get; set; }
        public Guid DepartmentId { get; set; }
        public Guid? OutreachingDepartmentId { get; set; }
        public Guid RoomId { get; set; }
        public Guid? FacultyMeetingRoomId { get; set; }
        public int? FacultyMeetingDuration { get; set; }
        public byte FacultyNoRequired { get; set; }
        public byte EmailSequence { get; set; }
        public Guid CourseFormatId { get; set; }
        public string ParticipantVideoFilename { get; set; }
        public string FeedbackSummaryFilename { get; set; }

        public DepartmentDto Department { get; set; }
        public DepartmentDto OutreachingDepartment { get; set; }
        public CourseFormatDto CourseFormat { get; set; }
        public RoomDto Room { get; set; }
        public RoomDto FacultyMeetingRoom { get; set; }

        public virtual ICollection<CourseParticipantDto> CourseParticipants { get; set; }
        public virtual ICollection<ScenarioDto> Scenarios { get; set; }
        public virtual ICollection<CourseScenarioFacultyRoleDto> CourseScenarioFacultyRoles { get; set; }
        public virtual ICollection<CourseSlotScenarioDto> CourseSlotScenarios { get; set; }
        public virtual ICollection<CourseSlotManequinDto> CourseSlotManequins { get; set; }
        public virtual ICollection<CourseSlotPresenterDto> CourseSlotPresenters { get; set; }
        public virtual ICollection<ChosenTeachingResourceDto> ChosenTeachingResources { get; set; }
    }
}
