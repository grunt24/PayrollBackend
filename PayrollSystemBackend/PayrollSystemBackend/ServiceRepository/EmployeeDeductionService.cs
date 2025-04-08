using System.Collections.Generic;
using System.Linq;
using PayrollSystem.DataAccessEFCore;
using PayrollSystemBackend.Core.Dto.Employee;
using PayrollSystemBackend.Core.Entities;
using PayrollSystemBackend.ServiceRepository.InterfaceRepository;

namespace PayrollSystemBackend.ServiceRepository
{
    public class EmployeeDeductionService : IEmployeeDeduction
    {
        private readonly ApplicationDbContext _context;

        public EmployeeDeductionService(ApplicationDbContext context)
        {
            _context = context;
        }

        // Add a new employee deduction
        public bool AddEmployeeDeduction(EmployeeDeduction deduction)
        {
            if (deduction == null)
            {
                return false; // Invalid deduction object
            }

            _context.EmployeeDeductions.Add(deduction); // Add the deduction
            _context.SaveChanges(); // Save changes to the database
            return true; // Successfully added
        }



        // Delete an employee deduction by id
        public void DeleteEmployeeDeduction(int id)
        {
            var deduction = _context.EmployeeDeductions.Find(id); // Find the deduction by id
            if (deduction != null)
            {
                deduction.IsDeleted = true; // Mark as deleted (soft delete)
                _context.EmployeeDeductions.Update(deduction);
                _context.SaveChanges(); // Save changes
            }
        }

        // Get all employee deductions
        public IEnumerable<EmployeeDeduction> GetAllEmployeeDeductions()
        {
            return _context.EmployeeDeductions.Where(d => !d.IsDeleted).ToList();
        }

        // Get a single employee deduction by id
        public EmployeeDeduction GetEmployeeDeductionById(int id)
        {
            if (id <= 0)
            {
                throw new KeyNotFoundException($"Employee with ID {id} not found.");
            }

            return _context.EmployeeDeductions.FirstOrDefault(d => d.Id == id && !d.IsDeleted);
        }

        // Update an employee deduction
        public void UpdateEmployeeDeduction(EmployeeDeductionRequest deduction, int id)
        {
            var existingDeduction = _context.EmployeeDeductions.Find(id);
            if (existingDeduction == null)
            {
                throw new KeyNotFoundException($"Employee Deduction with ID {id} not found.");
            }

            // Update fields
            existingDeduction.IsActive = deduction.IsActive ?? true;
            _context.SaveChanges(); // Save changes
        }

        public IEnumerable<EmployeeDeduction> GetEmployeeDeductionByEmployeeId(int employeeId)
        {
            return _context.EmployeeDeductions.Where(d => d.EmployeeId == employeeId).ToList();
        }

        public void UpdateEmployeeDeductionById(int employeeId, int deductionId, EmployeeDeduction updatedDeduction)
        {
            var deduction = _context.EmployeeDeductions
                .FirstOrDefault(d => d.EmployeeId == employeeId && d.Id == deductionId);

            if (deduction != null)
            {
                deduction.EmployeeDeductionName = updatedDeduction.EmployeeDeductionName;
                deduction.Amount = updatedDeduction.Amount;
                deduction.IsActive = updatedDeduction.IsActive;

                _context.SaveChanges();
            }
            else
            {
                throw new KeyNotFoundException($"Deduction with ID {deductionId} not found for Employee ID {employeeId}.");
            }
        }

        // ✅ Save changes for bulk updates
        public void SaveChanges()
        {
            _context.SaveChanges();
        }

    }
}
