using System.ComponentModel.DataAnnotations;

namespace OrganicPortalBackend.Models.Database.User.Recovery
{
    public class RecoveryModel
    {
        [Key]
        public long Id { get; set; }
        public DateTime CreatedDate { get; init; } = DateTime.UtcNow;


        [Required]
        public string Token { get; set; } = string.Empty; // *#* schema


        [Required]
        public string Code { get; set; } = string.Empty;

        [Required]
        public DateTime ExpiredDate { get; set; } = DateTime.UtcNow.AddMinutes(5);
    }
}
