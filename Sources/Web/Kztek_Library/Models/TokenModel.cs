namespace Kztek_Library.Models
{
    public class TokenModel
    {
        public string Identifier { get; set; }

        public int Expires_In { get; set; }

        public string Token { get; set; }
    }
}