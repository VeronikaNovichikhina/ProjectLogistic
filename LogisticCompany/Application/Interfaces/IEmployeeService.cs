using LogisticCompany.Application.DTO;
using LogisticCompany.Domain.Entities.Employee;
using LogisticCompany.DTO;
using System.Security.Claims;

namespace LogisticCompany.Application.Interfaces
{
    public interface IEmployeeService
    {
        Task<Employee> GetCurrentEmployeeAsync(ClaimsPrincipal user);

        Task ToggleEmployeeStatusAsync(int employeeId);

        Task<Employee?> GetEmployeeDetailsAsync(int employeeId);


        Task<List<Employee>> GetEmployeesAsync(
        int branchId,
        string position,
        string searchTerm);

        Task<Employee?> GetByIdAsync(int id);

        Task<CreateEmployeeResult> CreateAsync(EmployeeDTO dto);

        Task UpdateAsync(EmployeeDTO dto);

        Task DeleteAsync(int employeeId);

        Task<Employee> SaveEmployeeAsync(Employee employee);
        Task<Employee?> GetEmployeeWithBranchAsync(string email);

    }

}
