namespace Lucky.Models
{
    public class Employee
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public decimal Salary { get; set; }

        public string Email { get; set; }

        public string? ResetToken { get; set; }

        public DateTime? ResetTokenExpiry { get; set; }
    }
}