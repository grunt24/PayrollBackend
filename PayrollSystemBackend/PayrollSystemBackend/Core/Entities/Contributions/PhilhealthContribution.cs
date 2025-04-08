public class PhilHealthContribution
{
    public int Id { get; set; }
    public int Year { get; set; } = DateTime.Now.Year;
    public decimal MinSalary { get; set; }
    public decimal MaxSalary { get; set; }
    public decimal PremiumRate { get; set; }
    public decimal MonthlyPremium { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;

    public string SalaryRange => MinSalary == MaxSalary
        ? $"₱ {MinSalary:N2}"
        : $"₱ {MinSalary:N2} to ₱ {MaxSalary:N2}";

    public string MonthlyPremiumRange => MinSalary == MaxSalary
        ? $"₱{MonthlyPremium:N2}"
        : $"₱{(MinSalary * PremiumRate):N2} to ₱{(MaxSalary * PremiumRate):N2}";

}
