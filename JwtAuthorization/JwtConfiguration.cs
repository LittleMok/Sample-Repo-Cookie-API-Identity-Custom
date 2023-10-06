using System.ComponentModel.DataAnnotations;

namespace TestIdentity.JwtAuthorization
{
    public class JwtConfiguration
    {
        public bool SaveToken { get; set; }
        [Required]
        public string Audience { get; set; } = string.Empty;
        [Required]
        public string ValidIssuer { get; set; } = string.Empty;
        [Required]
        public string SigningKey { get; set; } = string.Empty;
        [Required]
        public int DefaultExpirationMinutes { get; set; }
    }
}
