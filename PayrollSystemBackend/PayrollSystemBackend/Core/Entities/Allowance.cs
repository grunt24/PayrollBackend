using payroll_system.Core.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace PayrollSystem.Domain.Entities
{
    public class Allowance : BaseEntity<int>
    {
        public string AllowanceName { get; set; }
        public decimal Amount { get; set; }
        public int? EmployeeId { get; set; }
        [JsonIgnore]
        [ForeignKey("EmployeeId")]
        public Employee? Employee { get; set; }

    }
}
