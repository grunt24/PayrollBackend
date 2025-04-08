using PayrollSystemBackend.Core.Entities;
using Microsoft.EntityFrameworkCore;
using PayrollSystemBackend.ServiceRepository.InterfaceRepository;
using PayrollSystem.DataAccessEFCore;
using PayrollSystemBackend.Core.Dto.Leave;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;

namespace PayrollSystemBackend.ServiceRepository
{
    public class LeaveService : ILeaveService
    {
        private readonly ApplicationDbContext _context;

        public LeaveService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<LeaveDto> AddLeaveAsync(LeaveDto leaveDto)
        {
            var newLeave = new Leave
            {
                EmployeeId = leaveDto.EmployeeId,
                LeaveDates = leaveDto.LeaveDates,
                IsPaid = leaveDto.IsPaid,
                Reason = leaveDto.Reason,
                CreatedAt = DateTime.UtcNow
            };

            _context.Leaves.Add(newLeave);
            await _context.SaveChangesAsync();

            return new LeaveDto
            {
                Id = newLeave.Id,
                EmployeeId = newLeave.EmployeeId,
                LeaveDates = newLeave.LeaveDates,
                IsPaid = newLeave.IsPaid,
                Reason = newLeave.Reason,
                CreatedAt = newLeave.CreatedAt
            };
        }

        public async Task<IEnumerable<LeaveDto>> GetAllLeavesAsync()
        {
            var leaves = await _context.Leaves.ToListAsync();
            return leaves.Select(l => new LeaveDto
            {
                Id = l.Id,
                EmployeeId = l.EmployeeId,
                LeaveDates = l.LeaveDates,
                IsPaid = l.IsPaid,
                Reason = l.Reason,
                CreatedAt = l.CreatedAt
            });
        }

        public async Task<LeaveDto?> GetLeaveByIdAsync(int id)
        {
            var leave = await _context.Leaves.FirstOrDefaultAsync(l => l.Id == id);
            if (leave == null) return null;

            return new LeaveDto
            {
                Id = leave.Id,
                EmployeeId = leave.EmployeeId,
                LeaveDates = leave.LeaveDates,
                IsPaid = leave.IsPaid,
                Reason = leave.Reason,
                CreatedAt = leave.CreatedAt
            };
        }

        public async Task<IEnumerable<LeaveDto>> GetLeavesByEmployeeIdAsync(int employeeId)
        {
            var leaves = await _context.Leaves
                .Where(l => l.EmployeeId == employeeId)
                .ToListAsync();

            return leaves.Select(l => new LeaveDto
            {
                Id = l.Id,
                EmployeeId = l.EmployeeId,
                LeaveDates = l.LeaveDates,
                IsPaid = l.IsPaid,
                Reason = l.Reason,
                CreatedAt = l.CreatedAt
            });
        }

        public async Task<bool> DeleteLeaveAsync(int id)
        {
            var leave = await _context.Leaves.FindAsync(id);
            if (leave == null)
            {
                return false;
            }

            _context.Leaves.Remove(leave);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UpdateLeaveAsync(int id, LeaveDto leaveDto)
        {
            var existingLeave = await _context.Leaves.FindAsync(id);
            if (existingLeave == null)
            {
                return false;
            }

            existingLeave.EmployeeId = leaveDto.EmployeeId;
            existingLeave.LeaveDates = leaveDto.LeaveDates;
            existingLeave.IsPaid = leaveDto.IsPaid;
            existingLeave.Reason = leaveDto.Reason;
            existingLeave.CreatedAt = leaveDto.CreatedAt;

            await _context.SaveChangesAsync();
            return true;
        }
    }
}
