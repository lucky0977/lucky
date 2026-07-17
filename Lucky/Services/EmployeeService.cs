using Lucky.DTOs;
using Lucky.Models;
using Lucky.Repositories;

namespace Lucky.Services
{
    public class EmployeeService : IEmployeeService
    {
        private readonly IEmployeeRepository _repository;

        public EmployeeService(IEmployeeRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<EmployeeResponseDto>> GetAllAsync()
        {
            var employees = await _repository.GetAllAsync();

            return employees.Select(e => new EmployeeResponseDto
            {
                Id = e.Id,
                Name = e.Name,
                Email = e.Email,
                Salary = e.Salary
            }).ToList();
        }

        public async Task<EmployeeResponseDto?> GetByIdAsync(int id)
        {
            var employee = await _repository.GetByIdAsync(id);
            if (employee == null) return null;

            return new EmployeeResponseDto
            {
                Id = employee.Id,
                Name = employee.Name,
                Email = employee.Email,
                Salary = employee.Salary
            };
        }

        public async Task<EmployeeResponseDto> CreateAsync(EmployeeCreateDto dto)
        {
            var employee = new Employee
            {
                Name = dto.Name,
                Email = dto.Email,
                Salary = dto.Salary,
                PasswordHash = "" // Note: employees created here have no login access
                                  // Real accounts should go through /api/Auth/register instead
            };

            await _repository.AddAsync(employee);
            await _repository.SaveChangesAsync();

            return new EmployeeResponseDto
            {
                Id = employee.Id,
                Name = employee.Name,
                Email = employee.Email,
                Salary = employee.Salary
            };
        }

        public async Task<EmployeeResponseDto?> UpdateAsync(int id, EmployeeUpdateDto dto)
        {
            var employee = await _repository.GetByIdAsync(id);
            if (employee == null) return null;

            employee.Name = dto.Name;
            employee.Email = dto.Email;
            employee.Salary = dto.Salary;

            await _repository.UpdateAsync(employee);
            await _repository.SaveChangesAsync();

            return new EmployeeResponseDto
            {
                Id = employee.Id,
                Name = employee.Name,
                Email = employee.Email,
                Salary = employee.Salary
            };
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var employee = await _repository.GetByIdAsync(id);
            if (employee == null) return false;

            await _repository.DeleteAsync(employee);
            await _repository.SaveChangesAsync();

            return true;
        }
    }
}