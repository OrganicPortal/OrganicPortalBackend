using System.ComponentModel.DataAnnotations;

namespace OrganicPortalBackend.Models.Database.User
{
    public class PasswordModel
    {
        [Key]
        public long Id { get; set; }
        public string Key { get; set; } = string.Empty; // *#* schema


        #region pass_info
        [Required]
        public string Password { get; set; } = string.Empty; // *#* schema
        #endregion


        [Required]
        public bool IsActive { get; set; } = false;
        public DateTime? DeactivateDate { get; set; } = null;
    }
}
