namespace Core.Utilities.Security.JWT
{
    public class AccessToken
    {
        public string Token { get; set; } // Sanal kartın şifreli metni
        public DateTime Expiration { get; set; } // Kartın son kullanma tarihi
    }
}