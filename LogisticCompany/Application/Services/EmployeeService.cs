using LogisticCompany.Application.DTO;
using LogisticCompany.Application.Interfaces;
using LogisticCompany.Db;
using LogisticCompany.Domain.Entities.Employee;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using System;
using System.Security.Claims;

namespace LogisticCompany.Application.Services
{
    public class EmployeeService: IEmployeeService
    {
        private readonly AppDbContext _db;
        private readonly IRoleHelper _roleHelper;
        private readonly IPasswordService _passwordService;
        public EmployeeService(AppDbContext db, IRoleHelper roleHelper, IPasswordService passwordService)
        {
            _db = db;
            _roleHelper = roleHelper;
            _passwordService = passwordService;
        }

        public async Task<Employee?> GetCurrentEmployeeAsync(ClaimsPrincipal user)
        {
            if (user.Identity == null || !user.Identity.IsAuthenticated)
                return null;


            return await _db.Employees
                .Include(e => e.Branch)
                    .ThenInclude(b => b.Town)
                        .ThenInclude(t => t.Country)
                .FirstOrDefaultAsync(e => e.Email == user.Identity.Name);
        }
        public async Task<List<Employee>> GetEmployeesAsync(
        int branchId,
        string position,
        string searchTerm)
        {
            var query = _db.Employees
                .Include(e => e.Branch)
                    .ThenInclude(b => b.Town)
                .AsQueryable();

            if (branchId > 0)
                query = query.Where(e => e.BranchId == branchId);

            if (!string.IsNullOrWhiteSpace(position))
                query = query.Where(e => e.Position == position);

            if (!string.IsNullOrWhiteSpace(searchTerm))
                query = query.Where(e =>
                    e.FirstName.Contains(searchTerm) ||
                    e.LastName.Contains(searchTerm) ||
                    e.Email.Contains(searchTerm) ||
                    e.Phone.Contains(searchTerm) ||
                    e.Branch.NameBranches.Contains(searchTerm));

            return await query
                .OrderBy(e => e.LastName)
                .ThenBy(e => e.FirstName)
                .ToListAsync();
        }

        public async Task<Employee?> GetByIdAsync(int id)
        {
            return await _db.Employees
                .Include(e => e.Branch)
                    .ThenInclude(b => b.Town)
                .Include(e => e.User)
                .FirstOrDefaultAsync(e => e.EmployeeId == id);
        }

        public async Task<CreateEmployeeResult> CreateAsync(EmployeeDTO dto)
        {
            if (await _db.Employees.AnyAsync(e => e.Email == dto.Email))
                throw new Exception("Email уже существует");

            if (await _db.Employees.AnyAsync(e => e.Phone == dto.Phone))
                throw new Exception("Телефон уже существует");

            using var tx = await _db.Database.BeginTransactionAsync();
            var result = new CreateEmployeeResult();

            var employee = new Employee
            {
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                PatronymicName = dto.PatronymicName,
                Email = dto.Email,
                Phone = dto.Phone,
                Position = dto.Position,
                BranchId = dto.BranchId
            };

            if (dto.CreateUserAccount)
            {
                var password = GenerateSecurePassword();
                var role = _roleHelper.GetRoleByPosition(dto.Position);
                var user = new User
                {
                    Email = dto.Email,
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword(password),
                    Role = role,
                    IsTemporaryPassword = true
                };

                _db.Users.Add(user);
                await _db.SaveChangesAsync();

                employee.UserId = user.Id;
                result.UserCreated = true;
                result.TemporaryPassword = password;
            }

            _db.Employees.Add(employee);
            await _db.SaveChangesAsync();
            await tx.CommitAsync();

            return result;
        }

        public async Task UpdateAsync(EmployeeDTO dto)
        {
            var employee = await _db.Employees
                .Include(e => e.User)
                .FirstOrDefaultAsync(e => e.EmployeeId == dto.EmployeeId);

            if (employee == null)
                throw new Exception("Сотрудник не найден");

            var oldPosition = employee.Position;

            employee.FirstName = dto.FirstName;
            employee.LastName = dto.LastName;
            employee.PatronymicName = dto.PatronymicName;
            employee.Phone = dto.Phone;
            employee.Position = dto.Position;
            employee.BranchId = dto.BranchId;

            if (employee.User != null && oldPosition != dto.Position)
            {
                employee.User.Role = _roleHelper.GetRoleByPosition(dto.Position);
            }

            await _db.SaveChangesAsync();
        }

        public async Task DeleteAsync(int employeeId)
        {
            using var tx = await _db.Database.BeginTransactionAsync();

            var employee = await _db.Employees
                .Include(e => e.User)
                .FirstOrDefaultAsync(e => e.EmployeeId == employeeId);

            if (employee == null)
                throw new Exception("Сотрудник не найден");

            if (employee.User != null)
                _db.Users.Remove(employee.User);

            _db.Employees.Remove(employee);
            await _db.SaveChangesAsync();

            await tx.CommitAsync();
        }
        private string GenerateSecurePassword(int length = 8) => _passwordService.Generate(length);


        public async Task ToggleEmployeeStatusAsync(int employeeId)
        {
            var employee = await _db.Employees
                .FirstOrDefaultAsync(e => e.EmployeeId == employeeId);

            if (employee == null)
                throw new Exception("Сотрудник не найден");


            await _db.SaveChangesAsync();
        }


        public async Task<Employee?> GetEmployeeDetailsAsync(int employeeId)
        {
            return await _db.Employees
                .Include(e => e.Branch)
                    .ThenInclude(b => b.Town)
                .Include(e => e.User)
                .FirstOrDefaultAsync(e => e.EmployeeId == employeeId);
        }


        public async Task<Employee> SaveEmployeeAsync(Employee employee)
        {
            // Валидация обязательных полей
            if (string.IsNullOrWhiteSpace(employee.FirstName))
                throw new ArgumentException("Имя обязательно для заполнения");

            if (string.IsNullOrWhiteSpace(employee.LastName))
                throw new ArgumentException("Фамилия обязательна для заполнения");

            if (string.IsNullOrWhiteSpace(employee.Phone))
                throw new ArgumentException("Телефон обязателен для заполнения");


            var employeeToUpdate = await _db.Employees
                .FirstOrDefaultAsync(e => e.EmployeeId == employee.EmployeeId);

            if (employeeToUpdate == null)
                throw new Exception("Сотрудник не найден");

            employeeToUpdate.FirstName = employee.FirstName;
            employeeToUpdate.LastName = employee.LastName;
            employeeToUpdate.PatronymicName = employee.PatronymicName;
            employeeToUpdate.Phone = employee.Phone;

            await _db.SaveChangesAsync();

            return employeeToUpdate;
        }

        public async Task<Employee?> GetEmployeeWithBranchAsync(string email)
        {
            return await _db.Employees
        .Include(e => e.User)   
        .Include(e => e.Branch)
            .ThenInclude(b => b.Town)
            .ThenInclude(t => t.Country)
        .FirstOrDefaultAsync(e => e.Email.ToLower() == email.ToLower());
        }
    }

}
