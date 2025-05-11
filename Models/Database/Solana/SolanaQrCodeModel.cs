using System.ComponentModel.DataAnnotations;

namespace OrganicPortalBackend.Models.Database.Solana
{
    public class SolanaQrCodeModel
    {
        // Базова інформація
        /* */
        [Key]
        public long Id { get; set; }
        public DateTime CreatedDate { get; init; } = DateTime.UtcNow;
        /* */


        // Інформація про qr код
        /* */
        [Required]
        public string QrBase64 { get; set; } = string.Empty;
        /* */


        // Список зв'язків в таблицях бд
        /* */
        [Required]
        [Range(1, long.MaxValue)]
        // Інформація про верифікований запси Solana
        public long SolanaSeedId { get; set; }
        public SolanaSeedModel? SolanaSeed { get; set; } = null;
        /* */
    }
}
