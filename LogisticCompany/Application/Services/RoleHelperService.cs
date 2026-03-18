using LogisticCompany.Application.Interfaces;

namespace LogisticCompany.Application.Services
{
    public class RoleHelperService: IRoleHelper
    {
        public string GetRoleByPosition(string position)
        {
            if (string.IsNullOrWhiteSpace(position))
                return "User";

            return position.Trim().ToLower() switch
            {
                "администратор" => "Admin",
                "менеджер" => "Manager",
                _ => "User"
            };
        }
    }
}
