using Microsoft.EntityFrameworkCore.Metadata.Internal;
using PayrollSystem.Domain.Entities;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace PayrollSystemBackend.Core.Dto.Employee
{
    public class EmployeeDto
    {
        public string? Name { get; set; }
        public string? Position { get; set; }
        public decimal HourlyRate { get; set; }
        public int DepartmentId { get; set; }
        [ForeignKey("DepartmentId")]
        public Department Department { get; set; }
    }
}
