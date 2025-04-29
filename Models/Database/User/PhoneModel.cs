using System.ComponentModel.DataAnnotations;

namespace OrganicPortalBackend.Models.Database.User
{
    public class PhoneModel
    {
        [Key]
        public long Id { get; set; }
        public DateTime CreatedDate { get; init; } = DateTime.UtcNow;
        public string Key { get; set; } = string.Empty; // *#* schema


        #region phone_info
        [Required]
        public string Phone { get; set; } = string.Empty; // *#* schema

        [Required]
        public string CountryCode { get; set; } = string.Empty; // *#* schema
        #endregion


        [Required]
        public bool IsActive { get; set; } = false;
        public DateTime? DeactivateDate { get; set; } = null;
    }
}
