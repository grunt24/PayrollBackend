using payroll_system.Core.Entities;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using PayrollSystem.DataAccessEFCore;
using PayrollSystemBackend.ServiceRepository.InterfaceRepository;

namespace payroll_system.Core.Services
{
    public class PositionService : IPositionService
    {
        private readonly ApplicationDbContext _context;

        public PositionService(ApplicationDbContext context)
        {
            _context = context;
        }

        public bool AddPosition(Position data)
        {
            var newPosition = new Position
            {
                PositionName = data.PositionName,
                WorkingDaysPerYear = data.WorkingDaysPerYear 
            };

            _context.Positions.Add(newPosition);
            _context.SaveChanges();
            return true;
        }

        public void DeletePosition(int id)
        {
            var position = _context.Positions
                                   .Include(p => p.Employees) // Load related employees
                                   .FirstOrDefault(p => p.Id == id);

            if (position == null)
            {
                return;
            }

            // Handle related employees
            if (position.Employees != null && position.Employees.Any())
            {
                foreach (var employee in position.Employees)
                {
                    // Option 1: Nullify the PositionId reference
                    employee.PositionId = null;

                    // Option 2: Assign to a default position (if applicable)
                    // employee.PositionId = defaultPositionId;
                }
            }

            // Remove the position
            _context.Positions.Remove(position);

            // Save changes to the database
            _context.SaveChanges();
        }

        public IEnumerable<Position> GetAllPositions()
        {
            return _context.Positions.AsNoTracking().ToList();
        }

        public Position GetPositionById(int id)
        {
            return _context.Positions.AsNoTracking().FirstOrDefault(p => p.Id == id);
        }

        public void UpdatePosition(Position position, int id)
        {
            var existingPosition = _context.Positions.Find(id);
            if (existingPosition == null)
            {
                throw new InvalidOperationException("Position not found.");
            }

            // Update fields
            existingPosition.PositionName = position.PositionName;
            existingPosition.WorkingDaysPerYear = position.WorkingDaysPerYear; // Update WorkingDaysPerYear

            // Explicitly mark the entity as modified to ensure EF Core tracks changes
            _context.Entry(existingPosition).State = EntityState.Modified;

            // Save changes
            _context.SaveChanges();
        }
    }
}
