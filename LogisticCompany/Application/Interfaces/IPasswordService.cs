namespace LogisticCompany.Application.Interfaces
{
    public interface IPasswordService
    {
        string Generate(int length = 8);
    }
}
