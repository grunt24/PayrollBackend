using Microsoft.EntityFrameworkCore;
using payroll_system.Core.Entities;
using PayrollSystem.DataAccessEFCore;
using PayrollSystem.Domain.Entities;
using PayrollSystemBackend.Core.Dto.General;
using PayrollSystemBackend.Core.Dto.Payroll;
using PayrollSystemBackend.Core.Dto.Payroll.EmployeeAllowancesDto;
using PayrollSystemBackend.Core.Dto.Payroll.PayslipPerPositionAndDepartment;
using PayrollSystemBackend.Core.Entities;
using PayrollSystemBackend.Migrations;
using PayrollSystemBackend.ServiceRepository.InterfaceRepository;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;

namespace PayrollSystemBackend
{
    public class PayrollService : IPayrollService
    {
        private readonly ApplicationDbContext _context;
        private readonly IPhilhealthContributionService philhealthService;

        public PayrollService(ApplicationDbContext context, IPhilhealthContributionService philhealthService )
        {
            _context = context;
            this.philhealthService = philhealthService;
        }

        public async Task<List<PayrollResult>> GeneratePayroll(PayrollAddRequestDto requestDto)
        {
            var startDate = requestDto.PayrollStartDate;
            var endDate = requestDto.PayrollEndDate;
            var payrollResults = new List<PayrollResult>();

            var employees = await _context.Employees
                .Include(e => e.Position)
                .Include(e => e.EmployeeDeductions)
                .Include(e => e.EmployeeSchedule)
                .Include(e => e.AcademicAward)
                .Include(e => e.Benefit)
                .Include(e => e.EducationalDegree)
                .Include(e => e.EmployeeAdditionalEarnings)
                .Where(e => requestDto.IdNumbers.Contains(e.IDNumber))
                .ToListAsync();

            var holidays = await _context.Calendars
                .Where(h => h.IsActive && h.HolidayDate >= startDate && h.HolidayDate <= endDate)
                .ToListAsync();

            var attendanceRecords = await _context.Attendances
                .Where(a => requestDto.IdNumbers.Contains(a.IDNumber) && a.Date.Date >= startDate && a.Date.Date <= endDate.AddDays(1))
                .ToListAsync();

            var leaveRecords = await _context.Leaves
    .Where(l => requestDto.IdNumbers.Contains(l.Employee.IDNumber))
    .ToListAsync();

            foreach (var employee in employees)
            {
                if (employee.EmployeeSchedule == null || !employee.EmployeeSchedule.Any())
                {
                    payrollResults.Add(new PayrollResult { Success = false, Error = new ErrorResponse($"No schedule found for Employee: {employee.FullName}", 400) });
                    continue;
                }

                int totalWorkingDays = 0, totalLatesMinutes = 0, totalUnderTimeMinutes = 0, totalAbsentDays = 0, totalLeaveDays = 0;
                decimal totalNightDifferentialPay = 0, totalNightDifferentialMinutes = 0, totalOvertimeMinutes = 0, overtimePay = 0;
                var totalLegalHolidayPay = 0m;
                var totalSpecialHolidayPay = 0m;

                List<string> absentDates = new List<string>();
                List<string> holidayDates = new List<string>();
                List<string> leaveDates = new List<string>();

                decimal ratePerDayFinal = Math.Round(employee.GetRatePerDay(), 2);
                decimal ratePerHourFinal = Math.Round(ratePerDayFinal / 8, 2);
                decimal ratePerMinute = Math.Round(ratePerHourFinal / 60, 2);
                
                for (var date = startDate; date <= endDate; date = date.AddDays(1))
                {
                    var dayOfWeek = (DayOfTheWeek)date.DayOfWeek;
                    var schedule = employee.EmployeeSchedule.FirstOrDefault(s => s.DayOfTheWeek == dayOfWeek);

                    if (schedule != null)
                    {
                        totalWorkingDays++;
                        var attendanceIn = attendanceRecords.FirstOrDefault(a => a.IDNumber == employee.IDNumber && a.Date.Date == date.Date && a.Status.ToLower() == "c/in");

                        // If shift ends the next day, check the next day's attendance for C/Out
                        var attendanceOut = attendanceRecords.FirstOrDefault(a => a.IDNumber == employee.IDNumber &&
                                                                                   ((a.Date.Date == date.Date && a.Status.ToLower() == "c/out") ||
                                                                                   (a.Date.Date == date.AddDays(1).Date && a.Status.ToLower() == "c/out")));

                        var allowedOvertime = schedule.AllowedOvertime;

                        var employeeLeave = leaveRecords.FirstOrDefault(l => l.Employee.IDNumber == employee.IDNumber &&
                                                                             l.LeaveDates.Any(ld => ld.Date == date.Date));

                        if (employeeLeave != null)
                        {
                            leaveDates.Add(date.ToString("MMMM dd, yyyy"));
                            totalLeaveDays++;
                            totalWorkingDays++; // ✅ Considered a working day

                            if (employeeLeave.IsPaid)
                            {
                                continue; // ✅ Paid leave: Do nothing further
                            }
                            else
                            {
                                // ❌ Unpaid Leave: Still a working day, but deduct from payroll
                                totalAbsentDays++;
                                absentDates.Add(date.ToString("MMMM dd, yyyy"));
                                continue;
                            }
                        }

                        var holidayList = holidays.Where(h => h.HolidayDate.Date == date.Date).ToList();
                        foreach (var holiday in holidayList)
                        {
                            holidayDates.Add(date.ToString("MMMM dd, yyyy"));

                            if (holiday.IsLegal.HasValue && holiday.IsLegal.Value)
                            {
                                totalLegalHolidayPay += ratePerDayFinal; // 100% additional pay
                            }
                            else
                            {
                                totalSpecialHolidayPay += ratePerDayFinal * 0.3m; // 30% additional pay
                            }
                        }

                        if (attendanceIn != null && attendanceOut != null)
                        {
                            if (attendanceIn.Time > schedule.ShiftStart)
                            {
                                totalLatesMinutes += (int)(attendanceIn.Time - schedule.ShiftStart).TotalMinutes;
                            }
                            if (attendanceOut.Time < schedule.ShiftEnd)
                            {
                                totalUnderTimeMinutes += (int)(schedule.ShiftEnd - attendanceOut.Time).TotalMinutes;
                            }

                            // 🔹 **Overtime Calculation with 30-Minute Rounding** 🔹
                            if (attendanceOut.Time > schedule.ShiftEnd)
                            {
                                TimeSpan overtimeWorked = attendanceOut.Time - schedule.ShiftEnd;

                                if (overtimeWorked > allowedOvertime)
                                {
                                    int rawOvertimeMinutes = (int)(overtimeWorked - allowedOvertime).TotalMinutes;
                                    totalOvertimeMinutes += rawOvertimeMinutes - (rawOvertimeMinutes % 30); // Round down to nearest 30 minutes
                                }
                            }

                            // Compute Night Differential (10 PM - 6 AM)
                            TimeSpan nightStart = new TimeSpan(22, 0, 0);
                            TimeSpan nightEnd = new TimeSpan(6, 0, 0);
                            TimeSpan nightHoursWorked = TimeSpan.Zero;

                            DateTime shiftStart = date.Date.Add(attendanceIn.Time);
                            DateTime shiftEnd = date.Date.Add(attendanceOut.Time);
                            if (attendanceOut.Time < attendanceIn.Time) // Crosses midnight
                            {
                                shiftEnd = shiftEnd.AddDays(1);
                            }

                            DateTime nightStartDateTime = date.Date.Add(nightStart);
                            DateTime nightEndDateTime = date.AddDays(1).Date.Add(nightEnd);

                            if (shiftEnd > nightStartDateTime) // Worked past 10 PM
                            {
                                DateTime nightWorkStart = shiftStart < nightStartDateTime ? nightStartDateTime : shiftStart;
                                DateTime nightWorkEnd = shiftEnd > nightEndDateTime ? nightEndDateTime : shiftEnd;
                                if (nightWorkEnd > nightWorkStart)
                                {
                                    nightHoursWorked += (nightWorkEnd - nightWorkStart);
                                }
                            }
                            if (nightHoursWorked.TotalMinutes > 0)
                            {
                                totalNightDifferentialMinutes += Math.Round((decimal)nightHoursWorked.TotalMinutes, 2); // Store as 119.56 instead of 119

                                decimal empRatePerDay = Math.Round(employee.GetRatePerDay(), 2);
                                decimal empRatePerHour = Math.Round(empRatePerDay / 8, 2);
                                decimal empRatePerMinute = Math.Round(empRatePerHour / 60, 2);

                                decimal totalWorkedPay = Math.Round(totalNightDifferentialMinutes * empRatePerMinute, 2);
                                decimal nightDifferentialPay = Math.Round(totalWorkedPay * 0.10m, 2);

                                totalNightDifferentialPay += nightDifferentialPay;
                            }


                        }
                        else
                        {
                            totalAbsentDays++;
                            absentDates.Add(date.ToString("MMMM dd, yyyy"));
                        }
                    }
                }



                employee.TotalAbsentDays += totalAbsentDays;
                _context.Employees.Update(employee);

                decimal basicPay = employee.BasicSalary ?? 0;
                if (totalOvertimeMinutes > 0)
                {
                    decimal overtimeRatePerMinute = ratePerHourFinal * 1.25m / 60; // 1.25x overtime rate
                    overtimePay = Math.Round(totalOvertimeMinutes * overtimeRatePerMinute, 2);
                }

                var sssContribution = await _context.SSSContributions
                    .FirstOrDefaultAsync(s => basicPay >= s.MinCompensation && basicPay <= s.MaxCompensation);

                decimal sssEmployeeShare = sssContribution?.TotalEmployeeContribution ?? 0;
                decimal sssEmployerShare = sssContribution?.TotalEmployerContribution ?? 0;

                var philHealth = await _context.PhilHealthContributions
                    .FirstOrDefaultAsync(p => basicPay >= p.MinSalary && basicPay <= p.MaxSalary);

                decimal computedPremium = philhealthService.ComputePremium(basicPay);

                decimal philHealthEmployeeShare = Math.Floor(computedPremium / 2 * 100) / 100;
                decimal philHealthEmployerShare = computedPremium - philHealthEmployeeShare;

                var pagibigContribution = await _context.PagibigContributions.FirstOrDefaultAsync();
                decimal pagibigEmployeeShare = pagibigContribution?.EmployeeContribution ?? 200;
                decimal pagibigEmployerShare = pagibigContribution?.EmployerContribution ?? 200;

                decimal totalEmployeeContributions = sssEmployeeShare + philHealthEmployeeShare + pagibigEmployeeShare;
                decimal totalEmployerContributions = sssEmployerShare + philHealthEmployerShare + pagibigEmployerShare;

                decimal totalContribution = totalEmployeeContributions + totalEmployerContributions;

                decimal deductionForLates = Math.Round(totalLatesMinutes * ratePerMinute, 2);
                decimal deductionForUnderTime = Math.Round(totalUnderTimeMinutes * ratePerMinute, 2);
                decimal deductionForAbsent = Math.Round(totalAbsentDays * ratePerDayFinal, 2);
                decimal totalDeductions = Math.Round(deductionForAbsent + deductionForLates + deductionForUnderTime + totalContribution + employee.CalculateEmployeeDeduction(), 2);
                //decimal totalDeductions = Math.Round(deductionForAbsent + deductionForLates + deductionForUnderTime + employee.CalculateEmployeeDeduction(), 2);

                decimal holidayPay = Math.Round(totalLegalHolidayPay + totalSpecialHolidayPay, 2);
                decimal grossSalary = Math.Round(
                    (ratePerDayFinal * totalWorkingDays) + holidayPay + totalNightDifferentialPay + overtimePay,
                    2
                );
                decimal netSalary = Math.Round(grossSalary - totalDeductions, 2);


                var payroll = new Payroll
                {
                    EmployeeId = employee.Id,
                    PayrollStartDate = startDate,
                    PayrollEndDate = endDate,
                    TotalWorkingDays = totalWorkingDays,
                    TotalLatesMinutes = totalLatesMinutes,
                    TotalUnderTimeMinutes = totalUnderTimeMinutes,
                    TotalAbsentDays = totalAbsentDays,
                    GrossSalary = grossSalary,
                    TotalDeductions = totalDeductions,
                    NetSalary = netSalary,
                    NightDifferentialPay = totalNightDifferentialPay,
                    TotalNightDifferentialMinutes = totalNightDifferentialMinutes,
                    AbsentDates = string.Join(", ", absentDates),
                    HolidayDates = string.Join(", ", holidayDates),

                    SSSEmployeeShare = sssEmployeeShare,
                    SSSEmployerShare = sssEmployerShare,

                    PhilHealthEmployeeShare = philHealthEmployeeShare,
                    PhilHealthEmployerShare = philHealthEmployerShare,

                    PagibigEmployeeShare = pagibigEmployeeShare,
                    PagibigEmployerShare = pagibigEmployerShare,

                    TotalEmployerContributions = totalEmployerContributions,
                    TotalEmployeeContributions = totalEmployeeContributions,

                    TotalContribution = totalContribution,
                    LeaveDates = string.Join(", ", leaveDates),

                };

                await _context.Payrolls.AddAsync(payroll);
                payrollResults.Add(new PayrollResult { Success = true });
            }

            await _context.SaveChangesAsync();
            return payrollResults;
        }

        public Payroll GetPayrollById(int payrollId)
        {
            return _context.Payrolls.Include(p => p.Employee).FirstOrDefault(p => p.Id == payrollId);
        }

        public IEnumerable<PayrollDto> GetAllPayrolls()
        {
            var payrolls = _context.Payrolls
                .Include(p => p.Employee)
                    .ThenInclude(e => e.Position)
                .Include(p => p.Employee)
                    .ThenInclude(e => e.Department)
                .Include(p => p.Employee)
                    .ThenInclude(e => e.AcademicAward)
                .Include(p => p.Employee)
                    .ThenInclude(e => e.Benefit)
                .Include(p => p.Employee)
                    .ThenInclude(e => e.EducationalDegree)
                .Include(p => p.Employee)
                    .ThenInclude(e => e.EmployeeAdditionalEarnings)
                .Include(p => p.Employee)
                    .ThenInclude(e => e.EmployeeDeductions)

                .AsNoTracking()
                .ToList(); // Fetch data first

            return payrolls.Select(p =>
            {
                decimal ratePerDay = Math.Round(p.Employee.GetRatePerDay(), 2);

                decimal ratePerHour = Math.Round(ratePerDay / 8, 2);
                decimal ratePerMinute = Math.Round(ratePerHour / 60, 2);
                decimal totalAbsentDeduction = Math.Round(ratePerDay * p.TotalAbsentDays, 2);
                decimal totalLatesDeduction = Math.Round(ratePerMinute * p.TotalLatesMinutes, 2);
                decimal totalUndertimeDeduction = Math.Round(ratePerMinute * p.TotalUnderTimeMinutes, 2);

                decimal totalLegalHolidayPay = 0;
                decimal totalSpecialHolidayPay = 0;
                List<string> holidayDates = new List<string>();

                if (!string.IsNullOrWhiteSpace(p.HolidayDates))
                {
                    var holidayMatches = Regex.Matches(p.HolidayDates, @"\b[A-Za-z]+ \d{1,2}, \d{4}\b");
                    var holidayList = holidayMatches.Select(m => DateTime.ParseExact(m.Value, "MMMM dd, yyyy", CultureInfo.InvariantCulture)).ToList();

                    foreach (var holidayDate in holidayList)
                    {
                        var holidayRecords = _context.Calendars.Where(h => h.HolidayDate.Date == holidayDate.Date).ToList();

                        foreach (var holiday in holidayRecords)
                        {
                            holidayDates.Add(holidayDate.ToString("MMMM dd, yyyy"));

                            if (holiday.IsLegal.HasValue && holiday.IsLegal.Value)
                            {
                                totalLegalHolidayPay += ratePerDay; // 100% additional pay
                            }
                            else
                            {
                                totalSpecialHolidayPay += ratePerDay * 0.3m; // 30% additional pay
                            }
                        }
                    }
                }
                decimal holidayPay = Math.Round(totalLegalHolidayPay + totalSpecialHolidayPay, 2);
                
                return new PayrollDto
                {
                    Id = p.Id,
                    fullName = p.Employee.FullName,
                    PayrollStartDate = p.PayrollStartDate.ToString("MMMM dd, yyyy"),
                    PayrollEndDate = p.PayrollEndDate.ToString("MMMM dd, yyyy"),
                    TotalWorkingDays = p.TotalWorkingDays,
                    TotalLatesMinutes = p.TotalLatesMinutes,
                    TotalUnderTimeMinutes = p.TotalUnderTimeMinutes,
                    TotalAbsentDays = p.TotalAbsentDays,
                    GrossSalary = p.GrossSalary,
                    TotalDeductions = p.TotalDeductions,
                    NetSalary = p.NetSalary,
                    BasicSalary = p.Employee.BasicSalary ?? 0,

                    // Employee Details
                    PositionName = p.Employee.Position?.PositionName,
                    DepartmentName = p.Employee.Department?.DepartmentName,
                    AcademicAwardName = p.Employee.AcademicAward?.AwardName,
                    AcademicAwardAmount = p.Employee.AcademicAward?.AwardAmount ?? 0,
                    BenefitName = p.Employee.Benefit?.BenefitName,
                    BenefitAmount = p.Employee.Benefit?.BenefitAmount ?? 0,
                    EducationalDegreeName = p.Employee.EducationalDegree?.AchievementName,
                    EducationalDegreeAmount = p.Employee.EducationalDegree?.AchievementAmount ?? 0,
                    DateHired = p.Employee.DateHired?.ToString("MMMM dd, yyyy"),
                    CreatedAt = p.CreatedAt,
                    PayrollStatus = p.Status.HasValue ? p.Status.Value.ToString() : "Pending",
                    AbsentDates = p.AbsentDates,
                    HolidayDates = string.Join(", ", holidayDates),

                    HolidayCount = holidayDates.Count,
                    TotalHolidayPay = holidayPay,
                    NightDifferentialPay = p.NightDifferentialPay,
                    TotalNightDifferentialMinutes = p.TotalNightDifferentialMinutes,

                    // Salary Breakdown
                    RatePerDay = (ratePerDay),
                    RatePerHour = ratePerHour,
                    RatePerMinute = ratePerMinute,

                    TotalAbsentDeduction = totalAbsentDeduction,
                    TotalLatesDeduction = totalLatesDeduction,
                    TotalUnderTimeDeduction = totalUndertimeDeduction,

                    SssEmployeeShare = p.SSSEmployeeShare,
                    SssEmployerShare = p.SSSEmployerShare,
                    
                    PhilHealthEmployeeShare = p.PhilHealthEmployeeShare,
                    PhilHealthEmployerShare = p.PhilHealthEmployerShare,
                    
                    PagibigEmployeeShare = p.PagibigEmployeeShare,
                    PagibigEmployerShare = p.PagibigEmployerShare,
                    
                    TotalEmployeeContributions = p.TotalEmployeeContributions,
                    TotalEmployerContributions = p.TotalEmployerContributions,
                    
                    TotalContribution = p.TotalContribution,
                    LeaveDates = p.LeaveDates,


                    EmployeeAllowances = new EmployeeAllowances
                    {
                        AcademicAwards = p.Employee.AcademicAward != null
                            ? new List<AcademicAward> { p.Employee.AcademicAward }
                            : new List<AcademicAward>(),

                        AdditionalEarning = p.Employee.EmployeeAdditionalEarnings?.ToList() ?? new List<Allowance>(),

                        Benefits = p.Employee.Benefit != null
                            ? new List<Benefit> { p.Employee.Benefit }
                            : new List<Benefit>(),

                        EducationalDegrees = p.Employee.EducationalDegree != null
                            ? new List<EducationalDegree> { p.Employee.EducationalDegree }
                            : new List<EducationalDegree>()
                    },

                    // Employee Deductions
                    EmployeeDeductions = p.Employee.EmployeeDeductions?
                        .Where(ed => ed.IsActive)
                        .Select(d => new EmployeeDeduction
                        {
                            Id = d.Id,
                            EmployeeDeductionName = d.EmployeeDeductionName,
                            Amount = d.Amount
                        }).ToList() ?? new List<EmployeeDeduction>()
                };
            }).ToList();
        }


        public async Task<IEnumerable<DepartmentPayrollDto>> GetPayrollsByDepartment()
        {
            var payrolls = await _context.Payrolls
                .Include(p => p.Employee)
                    .ThenInclude(e => e.Department)
                                .Include(p => p.Employee)
                    .ThenInclude(e => e.AcademicAward)
                .Include(p => p.Employee)
                    .ThenInclude(e => e.Benefit)
                .Include(p => p.Employee)
                    .ThenInclude(e => e.EducationalDegree)
                .Include(p => p.Employee)
                    .ThenInclude(e => e.EmployeeAdditionalEarnings)
                .Include(p => p.Employee)
                    .ThenInclude(e => e.EmployeeDeductions)
                .AsNoTracking()
                .ToListAsync();

            var groupedPayrolls = payrolls
                .GroupBy(p => p.Employee.Department.DepartmentName)
                .Select(g => new DepartmentPayrollDto
                {
                    DepartmentName = g.Key,
                    Payrolls = g.Select(p => new PayslipDto
                    {
                        PayrollNumber = p.Id,
                        IdNumber = p.Employee.IDNumber,
                        FullName = p.Employee.FullName,
                        PayrollStartDate = p.PayrollStartDate.ToString("MMMM dd, yyyy"),
                        PayrollEndDate = p.PayrollEndDate.ToString("MMMM dd, yyyy"),
                        GrossSalary = p.GrossSalary,
                        NetSalary = p.NetSalary,
                        TotalDeductions = p.TotalDeductions,
                        SssEmployeeShare = p.SSSEmployeeShare,
                        SssEmployerShare = p.SSSEmployerShare,
                        PhilHealthEmployeeShare = p.PhilHealthEmployeeShare,
                        PhilHealthEmployerShare = p.PhilHealthEmployerShare,
                        PagibigEmployeeShare = p.PagibigEmployeeShare,
                        PagibigEmployerShare = p.PagibigEmployerShare,
                        TotalEmployeeContributions = p.TotalEmployeeContributions,
                        TotalEmployerContributions = p.TotalEmployerContributions,
                        TotalContribution = p.TotalContribution,
                        EmployeeDeductions = p.Employee.EmployeeDeductions?
                        .Where(ed => ed.IsActive)
                        .Select(d => new EmployeeDeduction
                        {
                            Id = d.Id,
                            EmployeeDeductionName = d.EmployeeDeductionName,
                            Amount = d.Amount
                        }).ToList() ?? new List<EmployeeDeduction>(),
                    }).ToList()
                })
                .ToList();

            return groupedPayrolls;
        }

        public async Task<IEnumerable<PayslipDto>> GetPayrollsByEmployee(int employeeId)
        {
            var payrolls = await _context.Payrolls
                .Include(p => p.Employee)
                    .ThenInclude(e => e.Department)
                .Include(p => p.Employee)
                    .ThenInclude(e => e.AcademicAward)
                .Include(p => p.Employee)
                    .ThenInclude(e => e.Benefit)
                .Include(p => p.Employee)
                    .ThenInclude(e => e.EducationalDegree)
                .Include(p => p.Employee)
                    .ThenInclude(e => e.EmployeeAdditionalEarnings)
                .Include(p => p.Employee)
                    .ThenInclude(e => e.EmployeeDeductions)
                .Where(p => p.Employee.Id == employeeId)
                .AsNoTracking()
                .ToListAsync();

            return payrolls.Select(p => new PayslipDto
            {
                PayrollNumber = p.Id,
                IdNumber = p.Employee.IDNumber,
                FullName = p.Employee.FullName,
                PayrollStartDate = p.PayrollStartDate.ToString("MMMM dd, yyyy"),
                PayrollEndDate = p.PayrollEndDate.ToString("MMMM dd, yyyy"),
                GrossSalary = p.GrossSalary,
                NetSalary = p.NetSalary,
                TotalDeductions = p.TotalDeductions,
                SssEmployeeShare = p.SSSEmployeeShare,
                SssEmployerShare = p.SSSEmployerShare,
                PhilHealthEmployeeShare = p.PhilHealthEmployeeShare,
                PhilHealthEmployerShare = p.PhilHealthEmployerShare,
                PagibigEmployeeShare = p.PagibigEmployeeShare,
                PagibigEmployerShare = p.PagibigEmployerShare,
                TotalEmployeeContributions = p.TotalEmployeeContributions,
                TotalEmployerContributions = p.TotalEmployerContributions,
                TotalContribution = p.TotalContribution,
                EmployeeDeductions = p.Employee.EmployeeDeductions?
                    .Where(ed => ed.IsActive)
                    .Select(d => new EmployeeDeduction
                    {
                        Id = d.Id,
                        EmployeeDeductionName = d.EmployeeDeductionName,
                        Amount = d.Amount
                    }).ToList() ?? new List<EmployeeDeduction>()
            });
        }

        public async Task<IEnumerable<PositionPayrollDto>> GetPayrollsByPosition()
        {
            var payrolls = await _context.Payrolls
                .Include(p => p.Employee)
                    .ThenInclude(e => e.Position)
                                    .Include(p => p.Employee)
                    .ThenInclude(e => e.AcademicAward)
                .Include(p => p.Employee)
                    .ThenInclude(e => e.Benefit)
                .Include(p => p.Employee)
                    .ThenInclude(e => e.EducationalDegree)
                .Include(p => p.Employee)
                    .ThenInclude(e => e.EmployeeAdditionalEarnings)
                .Include(p => p.Employee)
                    .ThenInclude(e => e.EmployeeDeductions)
                .AsNoTracking()
                .ToListAsync();

            var groupedPayrolls = payrolls
                .GroupBy(p => p.Employee.Position.PositionName)
                .Select(g => new PositionPayrollDto
                {
                    PositionName = g.Key,
                    Payrolls = g.Select(p => new PayslipDto
                    {
                        PayrollNumber = p.Id,
                        IdNumber = p.Employee.IDNumber,
                        FullName = p.Employee.FullName,
                        PayrollStartDate = p.PayrollStartDate.ToString("MMMM dd, yyyy"),
                        PayrollEndDate = p.PayrollEndDate.ToString("MMMM dd, yyyy"),
                        GrossSalary = p.GrossSalary,
                        NetSalary = p.NetSalary,
                        TotalDeductions = p.TotalDeductions,
                        SssEmployeeShare = p.SSSEmployeeShare,
                        SssEmployerShare = p.SSSEmployerShare,
                        PhilHealthEmployeeShare = p.PhilHealthEmployeeShare,
                        PhilHealthEmployerShare = p.PhilHealthEmployerShare,
                        PagibigEmployeeShare = p.PagibigEmployeeShare,
                        PagibigEmployerShare = p.PagibigEmployerShare,
                        TotalEmployeeContributions = p.TotalEmployeeContributions,
                        TotalEmployerContributions = p.TotalEmployerContributions,
                        TotalContribution = p.TotalContribution,
                        EmployeeDeductions = p.Employee.EmployeeDeductions?
                        .Where(ed => ed.IsActive)
                        .Select(d => new EmployeeDeduction
                        {
                            Id = d.Id,
                            EmployeeDeductionName = d.EmployeeDeductionName,
                            Amount = d.Amount
                        }).ToList() ?? new List<EmployeeDeduction>(),
                    }).ToList()
                })
                .ToList();

            return groupedPayrolls;
        }

        public (decimal TotalNetSalary, decimal TotalDeductions, decimal TotalGrossIncome) GetPayrollSummary()
        {
            var totalNetSalary = _context.Payrolls.Sum(p => p.NetSalary);
            var totalDeduction = _context.Payrolls.Sum(p => p.TotalDeductions);
            var totalGrossSalary = _context.Payrolls.Sum(p => p.GrossSalary);

            return (totalNetSalary, totalDeduction, totalGrossSalary);
        }

        public async Task<Payroll?> UpdatePayrollStatusAsync(PayrollStatusRequest request)
        {
            if (request.PayrollId == null)
                throw new ArgumentNullException(nameof(request.PayrollId));

            var payroll = await _context.Payrolls.FindAsync(request.PayrollId);
            if (payroll == null)
                throw new KeyNotFoundException("Payroll not found.");

            if (!Enum.IsDefined(typeof(PayrollStatus), request.PayrollStatusId))
                throw new ArgumentException("Invalid payroll status.");

            payroll.Status = (PayrollStatus)request.PayrollStatusId;
            _context.Payrolls.Update(payroll);
            await _context.SaveChangesAsync();

            return payroll;
        }

        public async Task<PayrollStatusCountDto> GetPayrollStatusCount()
        {
            return new PayrollStatusCountDto
            {
                ApprovedCount = await _context.Payrolls.CountAsync(a => a.Status == PayrollStatus.Approved),
                RejectedCount = await _context.Payrolls.CountAsync(a => a.Status == PayrollStatus.Rejected),
                PendingCount = await _context.Payrolls.CountAsync(a => a.Status == PayrollStatus.Pending),
            };
        }

        public async Task<bool> DeletePayroll(int id)
        {
            var payroll = await _context.Payrolls.FindAsync(id);
            if (payroll == null)
            {
                return false;
            }

            _context.Remove(payroll);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<PayslipDto>> GetPayslipByPayrollId(int payrollId)
        {
            var payrolls = await _context.Payrolls
                .Include(p => p.Employee)
                    .ThenInclude(e => e.Department)
                .Include(p => p.Employee)
                    .ThenInclude(e => e.AcademicAward)
                .Include(p => p.Employee)
                    .ThenInclude(e => e.Benefit)
                .Include(p => p.Employee)
                    .ThenInclude(e => e.EducationalDegree)
                .Include(p => p.Employee)
                    .ThenInclude(e => e.EmployeeAdditionalEarnings)
                .Include(p => p.Employee)
                    .ThenInclude(e => e.EmployeeDeductions)
                .Where(p => p.Id == payrollId)
                .AsNoTracking()
                .ToListAsync();

            return payrolls.Select(p => new PayslipDto
            {
                PayrollNumber = p.Id,
                IdNumber = p.Employee.IDNumber,
                FullName = p.Employee.FullName,
                PayrollStartDate = p.PayrollStartDate.ToString("MMMM dd, yyyy"),
                PayrollEndDate = p.PayrollEndDate.ToString("MMMM dd, yyyy"),
                GrossSalary = p.GrossSalary,
                NetSalary = p.NetSalary,
                TotalDeductions = p.TotalDeductions,
                SssEmployeeShare = p.SSSEmployeeShare,
                SssEmployerShare = p.SSSEmployerShare,
                PhilHealthEmployeeShare = p.PhilHealthEmployeeShare,
                PhilHealthEmployerShare = p.PhilHealthEmployerShare,
                PagibigEmployeeShare = p.PagibigEmployeeShare,
                PagibigEmployerShare = p.PagibigEmployerShare,
                TotalEmployeeContributions = p.TotalEmployeeContributions,
                TotalEmployerContributions = p.TotalEmployerContributions,
                TotalContribution = p.TotalContribution,
                EmployeeDeductions = p.Employee.EmployeeDeductions?
                    .Where(ed => ed.IsActive)
                    .Select(d => new EmployeeDeduction
                    {
                        Id = d.Id,
                        EmployeeDeductionName = d.EmployeeDeductionName,
                        Amount = d.Amount
                    }).ToList() ?? new List<EmployeeDeduction>()
            });
        }
    }
}
