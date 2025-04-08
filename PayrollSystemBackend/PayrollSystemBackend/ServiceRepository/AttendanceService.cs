//using Microsoft.AspNetCore.Http.HttpResults;
//using Microsoft.EntityFrameworkCore;
//using payroll_system.Core.Entities;
//using PayrollSystem.DataAccessEFCore;
//using PayrollSystemBackend.Core.Entities;
//using PayrollSystemBackend.Core.Entities.Dto.General;
//using PayrollSystemBackend.InterfaceRepository;

//namespace PayrollSystemBackend.ServiceRepository
//{
//    public class AttendanceService : IAttendanceService
//    {
//        private readonly ApplicationDbContext _context;

//        public AttendanceService(ApplicationDbContext context)
//        {
//            _context = context;
//        }

//        public async Task<ServiceResponse<Attendance>> CreateAttendance(Attendance attendance)
//        {
//            if (attendance == null)
//            {
//                return new ServiceResponse<Attendance>(false, "Data is null.", null);
//            }
            
//            await _context.AddAsync(attendance);
//            await _context.SaveChangesAsync();

//            return new ServiceResponse<Attendance>(true, "Successfully Added", attendance);

//        }

//        public async Task<ServiceResponse<IEnumerable<Attendance>>> GetAll()
//        {
//            var listsAttendance = await _context.Attendances.ToListAsync();

//            if (!listsAttendance.Any())
//            {
//                return new ServiceResponse<IEnumerable<Attendance>>(false, "No attendances found.", new List<Attendance>());
//            }

//            return new ServiceResponse<IEnumerable<Attendance>>(true, "Attendances retrieved successfully.", listsAttendance);
//        }

//    }
//}
