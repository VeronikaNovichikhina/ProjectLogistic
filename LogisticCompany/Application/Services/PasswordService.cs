using System.Security.Cryptography;

namespace LogisticCompany.Application.Services
{
    public class PasswordService
    {
         const string сhars = "ABCDEFGHJKLMNPQRSTUVWXYZabcdefghjkmnpqrstuvwxyz23456789";

        public string Generate(int length = 5)
        {
            var random = new Random();
            return new string(Enumerable.Repeat(сhars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }
    }

}
