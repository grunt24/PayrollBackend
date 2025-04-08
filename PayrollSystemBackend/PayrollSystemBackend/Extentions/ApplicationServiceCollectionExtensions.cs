using payroll_system.Core.Services;
using PayrollSystemBackend.ServiceRepository;
using PayrollSystemBackend.ServiceRepository.InterfaceRepository;
using Microsoft.Extensions.DependencyInjection;

namespace PayrollSystemBackend.Extentions
{
    public static class ApplicationServiceCollectionExtensions
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services.AddScoped<IAllowanceService, AllowanceService>();
            services.AddScoped<IDepartmentService, DepartmentService>();
            services.AddScoped<IEmployeeService, EmployeeService>();
            services.AddScoped<IPayrollService, PayrollService>();
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IPositionService, PositionService>();
            services.AddScoped<IAcademicAwardService, AcademicAwardService>();
            services.AddScoped<IBenefitService, BenefitService>();
            services.AddScoped<IEducationalDegreeService, EducationalDegreeService>();
            services.AddScoped<IEmployeeDeduction, EmployeeDeductionService>();
            services.AddScoped<DashboardService>();
            services.AddScoped<IEmployeeScheduleService, EmployeeScheduleService>();
            services.AddScoped<ICalendarService, CalendarService>();
            services.AddScoped<IPhilhealthContributionService, PhilhealthContributionService>();
            services.AddScoped<ISSSContributionService, SSSContributionService>();
            services.AddScoped<IPagibigContributionService, PagibigContributionService>();
            services.AddScoped<ILeaveService, LeaveService>();

            return services;
        }
    }
}
