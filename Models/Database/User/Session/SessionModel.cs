using System.ComponentModel.DataAnnotations;

namespace OrganicPortalBackend.Models.Database.User.Session
{
    public class SessionModel
    {
        [Key]
        public long Id { get; set; }
        public string Key { get; set; } = string.Empty; // *#* schema
        public DateTime CreatedDate { get; init; } = DateTime.UtcNow;


        #region token_info
        [Required]
        public string Token { get; set; } = string.Empty; // *#* schema

        public DateTime ExpiredDate { get; set; } = DateTime.UtcNow.AddMinutes(ProgramSettings.TokenExpiredMinuts);

        public DateTime LastActivityDate { get; set; } = DateTime.UtcNow;
        #endregion
    }

    public class TokenInformation
    {
        public long UserId { get; set; } = 0;
        public DateTime CreatedDate { get; init; } = DateTime.UtcNow;
    }
}
