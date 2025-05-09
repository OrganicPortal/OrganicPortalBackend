using System.ComponentModel.DataAnnotations;

namespace OrganicPortalBackend.Models.Database.Solana
{
    public class SignatureModel
    {
        // Базова інформація
        /* */
        [Key]
        public long Id { get; set; }
        /* */


        // Інформація по сигнатурі
        /* */
        public string Signature { get; set; } = string.Empty;
        /* */


        // Список зв'язків в таблицях бд
        /* */
        [Required]
        [Range(1, long.MaxValue)]
        // Ідентифікатор запису соляни
        public long SolanaSeedId { get; set; }
        public SolanaSeedModel? SolanaSeed { get; set; } = null;
        /* */
    }
}
