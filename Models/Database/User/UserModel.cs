using System.ComponentModel.DataAnnotations;

namespace OrganicPortalBackend.Models.Database.User
{
    public class UserModel
    {
        [Key]
        public long Id { get; set; }
        public DateTime CreatedDate { get; init; } = DateTime.UtcNow;


        #region user_info
        [Required]
        [Length(2, 30)]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        [Length(2, 30)]
        public string MiddleName { get; set; } = string.Empty;

        [Required]
        [Length(2, 30)]
        public string LastName { get; set; } = string.Empty;
        #endregion
    }
}
