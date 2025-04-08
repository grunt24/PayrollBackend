using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using payroll_system.Core.Entities;
using PayrollSystem.Domain.Entities;
using PayrollSystemBackend.Core.Entities;
using PayrollSystemBackend.Core.Entities.Contributions;
using System.Reflection.Emit;

namespace PayrollSystem.DataAccessEFCore
{
    public class ApplicationDbContext : IdentityDbContext<IdentityUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }
        public DbSet<Department> Departments { get; set; }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<Payroll> Payrolls { get; set; }
        public DbSet<Allowance> Allowances { get; set; }
        public DbSet<Position> Positions { get; set; }
        public DbSet<EmployeeDeduction> EmployeeDeductions { get; set; }
        public DbSet<AcademicAward> AcademicAwards { get; set; }
        public DbSet<Benefit> Benefits { get; set; }
        public DbSet<EducationalDegree> EducationalDegrees { get; set; }
        public DbSet<EmployeeSchedule> EmployeeSchedules { get; set; }
        public DbSet<Attendance> Attendances { get; set; }
        public DbSet<BCASCalendar> Calendars { get; set; }

        //New
        public DbSet<Leave> Leaves { get; set; }
        public DbSet<PhilHealthContribution> PhilHealthContributions { get; set; }
        public DbSet<SSSContribution> SSSContributions { get; set; }
        public DbSet<PagibigContribution> PagibigContributions { get; set; }
        

        //public DbSet<SSSContribution> SSSContributions { get; set; }



        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
        }
    }
}
