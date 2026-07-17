using System.ComponentModel.DataAnnotations;

namespace Lucky.DTOs
{
    // What we SEND BACK to the client (safe — no password/token exposed)
    public class EmployeeResponseDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public decimal Salary { get; set; }
    }

    // What we ACCEPT when creating a new employee
    public class EmployeeCreateDto
    {
        [Required(ErrorMessage = "Name is required.")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Name must be between 2 and 100 characters.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        public string Email { get; set; }

        [Range(1, 10000000, ErrorMessage = "Salary must be a positive number.")]
        public decimal Salary { get; set; }
    }

    // What we ACCEPT when updating an employee
    public class EmployeeUpdateDto
    {
        [Required(ErrorMessage = "Name is required.")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Name must be between 2 and 100 characters.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        public string Email { get; set; }

        [Range(1, 10000000, ErrorMessage = "Salary must be a positive number.")]
        public decimal Salary { get; set; }
    }
}