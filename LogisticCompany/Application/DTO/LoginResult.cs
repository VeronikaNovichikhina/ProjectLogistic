namespace LogisticCompany.Application.DTO
{
    public class LoginResult
    {
        public bool IsSuccess { get; private set; }
        public string? Error { get; private set; }
        public string? Role { get; private set; }

        public static LoginResult Success(string role) =>
            new() { IsSuccess = true, Role = role };

        public static LoginResult Fail(string error) =>
            new() { IsSuccess = false, Error = error };
    }
}
