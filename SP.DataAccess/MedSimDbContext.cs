namespace SP.DataAccess
{
    using Microsoft.AspNet.Identity.EntityFramework;
    using Migrations;
    using SP.Metadata;
    using System;
    using System.Data.Entity;
    using System.Data.Entity.ModelConfiguration.Conventions;
    public partial class MedSimDbContext : IdentityDbContext<Participant, AspNetRole,
        Guid, AspNetUserLogin,AspNetUserRole, AspNetUserClaim>
    {
        public MedSimDbContext()
            : base("DefaultConnection")
        {
        }

        static MedSimDbContext()
        {
            Database.SetInitializer(new MigrateDatabaseToLatestVersion<MedSimDbContext, Configuration>());
        }
        public virtual DbSet<ActivityTeachingResource> ActivityTeachingResources { get; set; }
        public virtual DbSet<ChosenTeachingResource> ChosenTeachingResources { get; set; }
        public virtual DbSet<Culture> Cultures { get; set; }
        public virtual DbSet<Course> Courses { get; set; }
        public virtual DbSet<CourseActivity> CourseActivities { get; set; }
        public virtual DbSet<CourseFormat> CourseFormats { get; set; }
        public virtual DbSet<CourseParticipant> CourseParticipants { get; set; } 
        public virtual DbSet<CourseScenarioFacultyRole> CourseScenarioFacultyRoles { get; set; }
        public virtual DbSet<CourseSlot> CourseSlots { get; set; }
        public virtual DbSet<CourseSlotManequin> CourseSlotManequins { get; set; }
        public virtual DbSet<CourseSlotPresenter> CourseSlotPresenters { get; set; }
        public virtual DbSet<CourseSlotScenario> CourseSlotScenarios { get; set; }
        public virtual DbSet<CourseType> CourseTypes { get; set; }
        public virtual DbSet<CourseTypeScenarioRole> CourseTypeScenarioRoles { get; set; }
        public virtual DbSet<Department> Departments { get; set; }
        public virtual DbSet<FacultyScenarioRole> FacultyScenarioRoles { get; set; }
        public virtual DbSet<Institution> Institutions { get; set; }
        public virtual DbSet<Manequin> Manequins { get; set; }
        public virtual DbSet<ManequinManufacturer> ManequinManufacturers { get; set; }
        public virtual DbSet<ManequinModel> ManequinModels { get; set; }
        public virtual DbSet<ManequinService> ManequinServices { get; set; }
        public virtual DbSet<ProfessionalRole> ProfessionalRoles { get; set; }
        public virtual DbSet<ProfessionalRoleInstitution> ProfessionalRoleInstitutions { get; set; }
        public virtual DbSet<Room> Rooms { get; set; }
        public virtual DbSet<Scenario> Scenarios { get; set; }
        public virtual DbSet<ScenarioResource> ScenarioResources { get; set; }

        #region overrides //overriding to allow access without referencing aspnet.identity.entityframework assembly
        public override IDbSet<Participant> Users
        {
            get
            {
                return base.Users;
            }

            set
            {
                base.Users = value;
            }
        }
        #endregion //ovrerides


        public static MedSimDbContext Create()
        {
            return new MedSimDbContext();
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            //Configuration.LazyLoadingEnabled = false;
            //Configuration.ProxyCreationEnabled = false;

            modelBuilder.Conventions.Remove<ManyToManyCascadeDeleteConvention>();
            modelBuilder.Conventions.Add(new FixedLengthAttributeConvention());

            modelBuilder.Entity<ActivityTeachingResource>()
                .HasMany(e => e.ChosenTeachingResources)
                .WithRequired(e => e.ActivityTeachingResource)
                .HasForeignKey(e => e.ActivityTeachingResourceId);

            modelBuilder.Entity<ActivityTeachingResource>()
                .HasMany(e => e.ChosenTeachingResources)
                .WithRequired(e => e.ActivityTeachingResource)
                .HasForeignKey(e => e.ActivityTeachingResourceId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Culture>()
                .HasMany(e => e.Institutions)
                .WithRequired(e => e.Culture)
                .HasForeignKey(e => e.LocaleCode)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Course>()
                .HasMany(e => e.CourseParticipants)
                .WithRequired(e => e.Course)
                .HasForeignKey(e => e.CourseId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Course>()
                .HasMany(e => e.CourseScenarioFacultyRoles)
                .WithRequired(e => e.Course)
                .HasForeignKey(e => e.CourseId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Course>()
                .HasMany(e => e.CourseSlotScenarios)
                .WithRequired(e => e.Course)
                .HasForeignKey(e => e.CourseId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Course>()
                .HasMany(e => e.CourseSlotManequins)
                .WithRequired(e => e.Course)
                .HasForeignKey(e => e.CourseId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Course>()
                .HasMany(e => e.CourseSlotPresenters)
                .WithRequired(e => e.Course)
                .HasForeignKey(e => e.CourseId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Course>()
                .HasMany(e => e.ChosenTeachingResources)
                .WithRequired(e => e.Course)
                .HasForeignKey(e => e.CourseId)
                .WillCascadeOnDelete(false);

        modelBuilder.Entity<CourseActivity>()
                .HasMany(e => e.CourseSlots)
                .WithOptional(e => e.Activity)
                .HasForeignKey(e=>e.ActivityId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<CourseActivity>()
                .HasMany(e => e.ActivityChoices)
                .WithRequired(e => e.CourseActivity)
                .HasForeignKey(e => e.CourseActivityId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<CourseFormat>()
                .HasMany(e => e.CourseSlots)
                .WithRequired(e => e.CourseFormat)
                .HasForeignKey(e => e.CourseFormatId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<CourseFormat>()
                .HasMany(e => e.Courses)
                .WithRequired(e => e.CourseFormat)
                .HasForeignKey(e=>e.CourseFormatId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<CourseSlot>()
                .HasMany(e => e.ChosenTeachingResources)
                .WithRequired(e => e.CourseSlot)
                .HasForeignKey(e => e.CourseSlotId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<CourseSlot>()
                .HasMany(e => e.CourseSlotPresenters)
                .WithRequired(e => e.CourseSlot)
                .HasForeignKey(e => e.CourseSlotId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<CourseSlot>()
                .HasMany(e => e.CourseScenarioFacultyRoles)
                .WithRequired(e => e.CourseSlot)
                .HasForeignKey(e => e.CourseSlotId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<CourseSlot>()
                .HasMany(e => e.CourseSlotScenarios)
                .WithRequired(e => e.CourseSlot)
                .HasForeignKey(e => e.CourseSlotId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<CourseSlot>()
                .HasMany(e => e.CourseSlotManequins)
                .WithRequired(e => e.CourseSlot)
                .HasForeignKey(e => e.CourseSlotId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<CourseSlot>()
                .HasMany(e => e.ChosenTeachingResources)
                .WithRequired(e => e.CourseSlot)
                .HasForeignKey(e => e.CourseSlotId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<CourseType>()
                .HasOptional(e => e.InstructorCourse)
                .WithMany()
                .HasForeignKey(e => e.InstructorCourseId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<CourseType>()
                .HasMany(e => e.CourseTypeScenarioRoles)
                .WithRequired(e => e.CourseType)
                .HasForeignKey(e=>e.CourseTypeId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<CourseType>()
                .HasMany(e => e.CourseFormats)
                .WithRequired(e => e.CourseType)
                .HasForeignKey(e=>e.CourseTypeId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<CourseType>()
                .HasMany(e => e.Scenarios)
                .WithRequired(e => e.CourseType)
                .HasForeignKey(e => e.CourseTypeId);

            modelBuilder.Entity<CourseType>()
                .HasMany(e => e.CourseActivities)
                .WithRequired(e => e.CourseType)
                .HasForeignKey(e => e.CourseTypeId);

            modelBuilder.Entity<CourseType>()
                .HasMany(e => e.CourseTypeDepartments)
                .WithRequired(e => e.CourseType)
                .HasForeignKey(e => e.CourseTypeId);

            modelBuilder.Entity<Department>()
                .HasMany(e => e.CourseTypeDepartments)
                .WithRequired(e => e.Department)
                .HasForeignKey(e => e.DepartmentId);

            modelBuilder.Entity<Department>()
                .HasMany(e => e.Participants)
                .WithRequired(e => e.Department)
                .HasForeignKey(e=>e.DefaultDepartmentId);

            modelBuilder.Entity<Department>()
                .HasMany(e => e.Courses)
                .WithRequired(e => e.Department)
                .HasForeignKey(e => e.DepartmentId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Department>()
                .HasMany(e => e.OutreachCourses)
                .WithOptional(e => e.OutreachingDepartment)
                .HasForeignKey(e => e.OutreachingDepartmentId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Department>()
                .HasMany(e => e.Scenarios)
                .WithRequired(e => e.Department)
                .HasForeignKey(e => e.DepartmentId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Department>()
                .HasMany(e => e.Rooms)
                .WithRequired(e => e.Department)
                .HasForeignKey(e => e.DepartmentId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Department>()
                .HasMany(e => e.CourseParticipants)
                .WithRequired(e => e.Department)
                .HasForeignKey(e => e.DepartmentId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Department>()
                .HasMany(e => e.Manequins)
                .WithRequired(e => e.Department)
                .HasForeignKey(e => e.DepartmentId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<FacultyScenarioRole>()
                .HasMany(e => e.CourseScenarioFacultyRoles)
                .WithRequired(e => e.FacultyScenarioRole)
                .HasForeignKey(e => e.FacultyScenarioRoleId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<FacultyScenarioRole>()
                .HasMany(e => e.CourseTypeScenarioRoles)
                .WithRequired(e => e.FacultyScenarioRole)
                .HasForeignKey(e => e.FacultyScenarioRoleId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Institution>()
                .Property(e => e.LocaleCode)
                .IsFixedLength();

            modelBuilder.Entity<Institution>()
                .HasMany(e => e.Departments)
                .WithRequired(e => e.Institution)
                .HasForeignKey(e=>e.InstitutionId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Institution>()
                .HasMany(e => e.ProfessionalRoleInstitutions)
                .WithRequired(e => e.Institution)
                .HasForeignKey(e => e.InstitutionId);

            modelBuilder.Entity<Manequin>()
                .HasMany(e => e.ManequinServices)
                .WithRequired(e => e.Manequin)
                .HasForeignKey(e => e.ManequinId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Manequin>()
                .HasMany(e => e.CourseSlotManequins)
                .WithRequired(e => e.Manequin)
                .HasForeignKey(e => e.ManequinId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<ManequinManufacturer>()
                .HasMany(e => e.ManequinModels)
                .WithRequired(e => e.Manufacturer)
                .HasForeignKey(e => e.ManufacturerId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<ManequinModel>()
                .HasMany(e => e.Manequins)
                .WithRequired(e => e.Model)
                .HasForeignKey(e => e.ModelId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Participant>()
                .HasMany(e => e.CourseParticipants)
                .WithRequired(e => e.Participant)
                .HasForeignKey(e => e.ParticipantId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Participant>()
                .HasMany(e => e.CourseScenarioFacultyRoles)
                .WithRequired(e => e.Participant)
                .HasForeignKey(e => e.ParticipantId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Participant>()
                .HasMany(e => e.CourseSlotPresentations)
                .WithRequired(e => e.Participant)
                .HasForeignKey(e => e.ParticipantId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<ProfessionalRole>()
                .HasMany(e => e.Participants)
                .WithRequired(e => e.ProfessionalRole)
                .HasForeignKey(e => e.DefaultProfessionalRoleId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<ProfessionalRole>()
                .HasMany(e => e.CourseParticipants)
                .WithRequired(e => e.ProfessionalRole)
                .HasForeignKey(e => e.ProfessionalRoleId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<ProfessionalRole>()
                .HasMany(e => e.ProfessionalRoleInstitutions)
                .WithRequired(e => e.ProfessionalRole)
                .HasForeignKey(e => e.ProfessionalRoleId);

            modelBuilder.Entity<Room>()
                .HasMany(e => e.Courses)
                .WithRequired(e => e.Room)
                .HasForeignKey(e => e.RoomId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Room>()
                .HasMany(e => e.FacultyMeetings)
                .WithOptional(e => e.FacultyMeetingRoom)
                .HasForeignKey(e => e.FacultyMeetingRoomId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Scenario>()
                .HasMany(e => e.CourseSlotScenarios)
                .WithRequired(e => e.Scenario)
                .HasForeignKey(e=>e.ScenarioId)
                .WillCascadeOnDelete(false);

            base.OnModelCreating(modelBuilder);
        }

        /*
        private static void buildAuth(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AspNetUser>()
                .HasMany(e => e.Roles)
                .WithRequired(e => e.User)
                .HasForeignKey(e => e.UserId);

            modelBuilder.Entity<AspNetUser>()
                .HasMany(e => e.Claims)
                .WithRequired(e => e.User)
                .HasForeignKey(e => e.UserId);

            modelBuilder.Entity<AspNetUser>()
                .HasMany(e => e.Logins)
                .WithRequired(e => e.User)
                .HasForeignKey(e => e.UserId);

            modelBuilder.Entity<AspNetRole>()
                .HasKey(e=>e.Id)
                .HasMany(e => e.Users)
                .WithRequired(e => e.Role)
                .HasForeignKey(e => e.RoleId);

            modelBuilder.Entity<AspNetUserClaim>()
                .HasKey(e => e.Id);

            //suspect chances of the oauth provider keys overlapping is pretty remote
            modelBuilder.Entity<AspNetUserLogin>()
                .HasKey(e => new { e.LoginProvider, e.ProviderKey, e.UserId});

            modelBuilder.Entity<AspNetUserRole>()
                .HasKey(e => new { e.RoleId, e.UserId });

        }
        */

        private SanitizeStringProperties _sanitizeHtml { get; set; }
        private SanitizeStringProperties SanitizeHtml
        {
            get { return _sanitizeHtml ?? (_sanitizeHtml = new SanitizeStringProperties()); }
        }

        public override int SaveChanges()
        {
            SanitizeHtml.ForEntities(ChangeTracker);

            return base.SaveChanges();
        }


    }
}
