namespace Core.Utilities.Security.JWT
{
    public interface ITokenHelper
    {
        // User tablosunu tanımadığımız için, bilgileri string olarak dışarıdan isteyeceğiz
        AccessToken CreateToken(string id, string email, string role, string name);
    }
}