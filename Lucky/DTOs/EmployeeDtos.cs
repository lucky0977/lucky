namespace Lucky.DTOs
{
    
    public class EmployeeResponseDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public decimal Salary { get; set; }
    }

   
    public class EmployeeCreateDto
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public decimal Salary { get; set; }
    }

    
    public class EmployeeUpdateDto
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public decimal Salary { get; set; }
    }
}