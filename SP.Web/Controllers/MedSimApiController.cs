using System.Linq;
using System.Web.Http;
using Breeze.WebApi2;
using Newtonsoft.Json.Linq;
using Microsoft.AspNet.Identity;
using System.Security.Claims;
using System;
using SP.Dto;
using Breeze.ContextProvider;
using System.Web.Http.OData.Query;
using SP.Web.Controllers.Helpers;
using Microsoft.Data.OData.Query.SemanticAst;

namespace SP.Web.Controllers
{
    [BreezeController]
    [Authorize]
    public class MedSimApiController : ApiController
    {
        // Todo: inject via an interface rather than "new" the concrete class
        private MedSimDtoRepository _repository; // not populating in constructor as I believe this may be too early
        private MedSimDtoRepository Repo
        {
            get
            {
                return _repository ?? (_repository = new MedSimDtoRepository(User));
            }
        }


        [HttpPost]
        public SaveResult SaveChanges(JObject saveBundle)
        {
            return Repo.SaveChanges(saveBundle);
        }
        [HttpGet, EnableBreezeQuery]
		public IQueryable<ActivityTeachingResourceDto> ActivityTeachingResources(ODataQueryOptions options)
        {
            var iso = new IncludeSelectOptions(options);
            return Repo.ActivityTeachingResources(iso.Includes, iso.Selects, IncludeSelectOptions.Seperator);
        }
        [HttpGet, EnableBreezeQuery]
		public IQueryable<ParticipantDto> Participants(ODataQueryOptions options)
        {
            var iso = new IncludeSelectOptions(options);
            return Repo.GetParticipants(iso.Includes, iso.Selects, IncludeSelectOptions.Seperator);
        }
        [HttpGet, EnableBreezeQuery(MaxExpansionDepth = 3)]
        public IQueryable<CultureDto> Cultures(ODataQueryOptions options)
        {
            var iso = new IncludeSelectOptions(options);
            return Repo.GetCultures(iso.Includes, iso.Selects, IncludeSelectOptions.Seperator); ;
        }
        [HttpGet, EnableBreezeQuery]
        public IQueryable<CourseActivityDto> CourseActivities(ODataQueryOptions options) {
            var iso = new IncludeSelectOptions(options);
            return Repo.GetCourseActivities(iso.Includes, iso.Selects, IncludeSelectOptions.Seperator);
        }
        [HttpGet]
		public IQueryable<DepartmentDto> Departments(){ return Repo.Departments; }

        [HttpGet, EnableBreezeQuery(MaxExpansionDepth = 4)]
        public IQueryable<InstitutionDto> Institutions(ODataQueryOptions options)
        {
            var iso = new IncludeSelectOptions(options);
            return Repo.GetInstitutions(iso.Includes, iso.Selects, IncludeSelectOptions.Seperator);
        }
        [HttpGet]
		public IQueryable<FacultyScenarioRoleDto> SenarioRoles(){ return Repo.SenarioRoles; } 
        [HttpGet]
		public IQueryable<ManequinDto> Manequins(){ return Repo.Manequins; }
        [HttpGet]
        public IQueryable<ManequinModelDto> ManequinModels() { return Repo.ManequinModels; }
        [HttpGet]
		public IQueryable<ProfessionalRoleDto> ProfessionalRoles(){ return Repo.ProfessionalRoles; }
        [HttpGet]
        public IQueryable<ProfessionalRoleInstitutionDto> ProfessionalRoleInstitutions() { return Repo.ProfessionalRoleInstitutions; }
        [HttpGet]
		public IQueryable<ScenarioDto> Scenarios()
        {
            return Repo.Scenarios;
        }

        [HttpGet, EnableBreezeQuery]
        public IQueryable<CourseSlotDto> CourseSlots(ODataQueryOptions options)
        {
            var iso = new IncludeSelectOptions(options);
            return Repo.GetCourseSlots(iso.Includes, iso.Selects, IncludeSelectOptions.Seperator);
        }

        [HttpGet, EnableBreezeQuery]
        public IQueryable<CourseFormatDto> CourseFormats(ODataQueryOptions options)
        {
            var iso = new IncludeSelectOptions(options);
            return Repo.GetCourseFormats(iso.Includes, iso.Selects, IncludeSelectOptions.Seperator);
        }

        [HttpGet]
		public IQueryable<ScenarioResourceDto> ScenarioResources()
        {
            return Repo.ScenarioResources;
        }

        [HttpGet]
        public IQueryable<CourseTypeScenarioRoleDto> CourseTypeScenarioRoles()
        {
            return Repo.CourseTypeScenarioRoles;
        }

        [HttpGet]
        public IQueryable<FacultyScenarioRoleDto> FacultyScenarioRoles()
        {
            return Repo.FacultyScenarioRoles;
        }

        [HttpGet, EnableBreezeQuery(MaxExpansionDepth = 4)]
		public IQueryable<CourseDto> Courses(ODataQueryOptions options)
        {
            var iso = new IncludeSelectOptions(options);
            var returnVar = Repo.GetCourses(iso.Includes, iso.Selects, IncludeSelectOptions.Seperator);
            //hack alert - currently options.applyto does not handle includes which are deeper than a null parent
            //the following hack works around this for a very specific use scenario
            //this is about as ugly as hacks get, so steel yourself before reading ahead
            if (iso.Includes != null && iso.Includes.Any(i=>i.Contains("Activity/")))
            {
                var exp = (BinaryOperatorNode)options.Filter.FilterClause.Expression;
                System.Diagnostics.Debug.Assert(exp.OperatorKind == Microsoft.Data.OData.Query.BinaryOperatorKind.Equal);
                System.Diagnostics.Debug.Assert(((SingleValuePropertyAccessNode)exp.Left).Property.Name == "Id");
                Guid id = (Guid)((ConstantNode)exp.Right).Value;
                //todo fix this method up so that the query isn't being applied twice
                return returnVar.Where(c=>c.Id==id).ToList().AsQueryable();
            }
            return returnVar;
        }

        private class IncludeSelectOptions
        {
            public IncludeSelectOptions(ODataQueryOptions options)
            {
                var se = options.SelectExpand;
                if (se != null)
                {
                    if (se.RawExpand != null)
                    {
                        Includes =  se.RawExpand.Split(splitter);
                    }
                     if (se.RawSelect != null)
                    {
                        Selects = se.RawSelect.Split(splitter);
                    }
                }
                if (options.Filter != null)
                {
                    var anyAlls = FindAnyAllFilterOptions.GetPaths(options.Filter, Seperator.ToString());
                    if (anyAlls.Any())
                    {
                        Includes = (Includes ?? new string[0]).Concat(anyAlls).ToArray();
                    }
                }
                if (options.OrderBy != null)
                {
                    Selects = (Selects ?? new string[0]).Union(options.OrderBy.RawValue.Split(splitter)).ToArray();
                }
            }
            public readonly string[] Includes;
            public readonly string[] Selects;
            public const char Seperator = '/';
            const char splitter = ',';
        }
        [HttpGet, EnableBreezeQuery(MaxExpansionDepth = 3)]
        public IQueryable<CourseTypeDto> CourseTypes(ODataQueryOptions options)
        {
            var iso = new IncludeSelectOptions(options);
            return Repo.GetCourseTypes(iso.Includes, iso.Selects, IncludeSelectOptions.Seperator);
        }

        [HttpGet]
        public LookupBundle Lookups()
        {
            return new LookupBundle
            {
                Institutions = Repo.GetInstitutions(includes: new[] { "Departments.Rooms", "ProfessionalRoleInstitutions.ProfessionalRole", "Culture", "Departments.Manequins" }).ToList(),
                CourseTypes = Repo.GetCourseTypes().ToList(),
                ProfessionalRoles = Repo.ProfessionalRoles.ToList(),
                ManequinManufacturers = Repo.ManequinManufacturers.ToList(),
                Manequins = Repo.Manequins.ToList()
            };
        }
        
        // Diagnostic
        [HttpGet]
        public string Ping()
        {
            return "pong";
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_repository != null)
                {
                    _repository.Dispose();
                }
            }
            base.Dispose(disposing);
        }
    }
}