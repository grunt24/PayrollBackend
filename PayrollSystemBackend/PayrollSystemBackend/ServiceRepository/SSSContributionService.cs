using ExcelDataReader;
using Microsoft.EntityFrameworkCore;
using PayrollSystem.DataAccessEFCore;
using PayrollSystemBackend.Core.Entities.Contributions;
using PayrollSystemBackend.ServiceRepository.InterfaceRepository;

namespace PayrollSystemBackend.ServiceRepository
{
    public class SSSContributionService : ISSSContributionService
    {
        private readonly ApplicationDbContext _context;

        public SSSContributionService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<bool> ImportFromExcelAsync(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return false;

            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
            using var stream = file.OpenReadStream();
            using var reader = ExcelReaderFactory.CreateReader(stream);

            var sssContributions = new List<SSSContribution>();
            reader.Read(); // Skip the header row

            while (reader.Read())
            {
                if (reader.FieldCount < 8) continue; // Ensure required columns exist

                var minCompensation = Convert.ToDecimal(reader.GetValue(0) ?? 0);
                var maxCompensation = Convert.ToDecimal(reader.GetValue(1) ?? 0);
                var employerSS = Convert.ToDecimal(reader.GetValue(2) ?? 0);
                var employerMPF = Convert.ToDecimal(reader.GetValue(3) ?? 0);
                var employerEC = Convert.ToDecimal(reader.GetValue(4) ?? 0);
                var employeeSS = Convert.ToDecimal(reader.GetValue(6) ?? 0);
                var employeeMPF = Convert.ToDecimal(reader.GetValue(7) ?? 0);

                sssContributions.Add(new SSSContribution
                {
                    MinCompensation = minCompensation,
                    MaxCompensation = maxCompensation,
                    EmployerSS = employerSS,
                    EmployerMPF = employerMPF,
                    EmployerEC = employerEC,
                    EmployeeSS = employeeSS,
                    EmployeeMPF = employeeMPF,
                    Year = DateTime.Now.Year,
                    CreatedAt = DateTime.Now
                });
            }

            await _context.SSSContributions.AddRangeAsync(sssContributions);
            await _context.SaveChangesAsync();
            return true;
        }
        public async Task<IEnumerable<SSSContribution>> GetAllAsync()
        {
            return await _context.SSSContributions.ToListAsync();
        }

        public async Task<SSSContribution?> GetBySalaryAsync(decimal salary)
        {
            return await _context.SSSContributions
                .FirstOrDefaultAsync(e => salary >= e.MinCompensation && salary <= e.MaxCompensation);
        }

        public async Task<bool> DeleteAllAsync()
        {
            var allContributions = await _context.SSSContributions.ToListAsync();
            if (!allContributions.Any()) return false;

            _context.SSSContributions.RemoveRange(allContributions);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
