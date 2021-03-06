using SP.Metadata;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
namespace SP.Dto
{
    public abstract class ResourceDto 
	{
        public Guid Id { get; set; }
        public string Description { get; set; }
        public string ResourceFilename { get; set; }
	}
    [MetadataType(typeof(ResourceMetadata))]
    public class ActivityTeachingResourceDto : ResourceDto
    {
        public Guid CourseActivityId { get; set; }
        public CourseActivityDto CourseActivity { get; set; }

        public virtual ICollection<ChosenTeachingResourceDto> ChosenTeachingResources { get; set; }
    }
    [MetadataType(typeof(ResourceMetadata))]
    public class ScenarioResourceDto : ResourceDto
    {
        public Guid ScenarioId { get; set; }
        public ScenarioDto Scenario { get; set; }
    }
}
