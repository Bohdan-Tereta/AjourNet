using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AjourBT.Domain.Entities;
using AjourBT.Domain.Concrete;

namespace IntegrationTests
{
    public partial class AjourBTForTestContext: DbContext
    {
        public AjourBTForTestContext()
            : base("name=AjourBTConnection") { }


        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<BusinessTrip>()
                .HasRequired(u => u.Location)
                .WithMany(b => b.BusinessTrips)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Employee>()
               .HasOptional(u => u.Department)
               .WithMany(b => b.Employees)
               .WillCascadeOnDelete(false);

            modelBuilder.Entity<BusinessTrip>()
              .HasRequired(u => u.BTof)
              .WithMany(b => b.BusinessTrips)
              .WillCascadeOnDelete(false);

            modelBuilder.Entity<Employee>()
              .HasOptional(v => v.Visa)
              .WithRequired(e => e.VisaOf);
        }

        public DbSet<Employee> Employees { get; set; }
        public DbSet<Department> Departments { get; set; }
        public DbSet<Visa> Visas { get; set; }
        public DbSet<VisaRegistrationDate> VisaRegistrationDates { get; set; }
        public DbSet<BusinessTrip> BusinessTrips { get; set; }
        public DbSet<Location> Locations { get; set; }
        public DbSet<UserProfile> UserProfiles { get; set; }
        public DbSet<Permit> Permits { get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<Passport> Passports { get; set; }
        public DbSet<PrivateTrip> PrivateTrips { get; set; }
        public DbSet<Position> Positions { get; set; }
        public DbSet<Country> Countries { get; set; }
        public DbSet<Holiday> Holidays { get; set; }
        public DbSet<Journey> Journeys { get; set; }
        public DbSet<CalendarItem> CalendarItems { get; set; }
        public DbSet<Unit> Units { get; set; }
        public DbSet<Overtime> Overtimes { get; set; }
        public DbSet<Vacation> Vacations { get; set; }
        public DbSet<Sickness> Sicknesses { get; set; }
        public DbSet<Greeting> Greetings { get; set; }
        public DbSet<Insurance> Insurances { get; set; }
        public DbSet<QuestionSet> QuestionSets { get; set; }
        public DbSet<Questionnaire> Questionnaire { get; set; }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }
    }


}
