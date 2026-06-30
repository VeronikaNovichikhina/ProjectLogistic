using LogisticCompany.Application.Interfaces;
using System.Security.Cryptography;

namespace LogisticCompany.Application.Services
{
    public class PasswordService : IPasswordService
    {
        private const string Chars = "ABCDEFGHJKLMNPQRSTUVWXYZabcdefghjkmnpqrstuvwxyz23456789";
        public string Generate(int length = 8)
        {
            var result = new char[length];
            var bytes = new byte[length];
            RandomNumberGenerator.Fill(bytes);
            for (int i = 0; i < length; i++)
                result[i] = Chars[bytes[i] % Chars.Length];
            return new string(result);
        }
    }
}
