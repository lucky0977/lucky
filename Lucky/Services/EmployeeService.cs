using Lucky.DTOs;
using Lucky.Models;
using Lucky.Repositories;

namespace Lucky.Services
{
    public class EmployeeService : IEmployeeService
    {
        private readonly IEmployeeRepository _repository;
        private readonly ILogger<EmployeeService> _logger;

        public EmployeeService(IEmployeeRepository repository, ILogger<EmployeeService> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        public async Task<List<EmployeeResponseDto>> GetAllAsync()
        {
            _logger.LogInformation("Fetching all employees.");
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
            _logger.LogInformation("Fetching employee with Id {EmployeeId}.", id);
            var employee = await _repository.GetByIdAsync(id);

            if (employee == null)
            {
                _logger.LogWarning("Employee with Id {EmployeeId} not found.", id);
                return null;
            }

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
            _logger.LogInformation("Creating new employee with Email {Email}.", dto.Email);

            var employee = new Employee
            {
                Name = dto.Name,
                Email = dto.Email,
                Salary = dto.Salary,
                PasswordHash = ""
            };

            await _repository.AddAsync(employee);
            await _repository.SaveChangesAsync();

            _logger.LogInformation("Employee created successfully with Id {EmployeeId}.", employee.Id);

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
            _logger.LogInformation("Updating employee with Id {EmployeeId}.", id);
            var employee = await _repository.GetByIdAsync(id);

            if (employee == null)
            {
                _logger.LogWarning("Update failed: Employee with Id {EmployeeId} not found.", id);
                return null;
            }

            employee.Name = dto.Name;
            employee.Email = dto.Email;
            employee.Salary = dto.Salary;

            await _repository.UpdateAsync(employee);
            await _repository.SaveChangesAsync();

            _logger.LogInformation("Employee with Id {EmployeeId} updated successfully.", id);

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
            _logger.LogInformation("Deleting employee with Id {EmployeeId}.", id);
            var employee = await _repository.GetByIdAsync(id);

            if (employee == null)
            {
                _logger.LogWarning("Delete failed: Employee with Id {EmployeeId} not found.", id);
                return false;
            }

            await _repository.DeleteAsync(employee);
            await _repository.SaveChangesAsync();

            _logger.LogInformation("Employee with Id {EmployeeId} deleted successfully.", id);

            return true;
        }
    }
}