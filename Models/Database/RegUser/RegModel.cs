using System.ComponentModel.DataAnnotations;

namespace OrganicPortalBackend.Models.Database.RegUser
{
    public class RegModel
    {
        [Key]
        public long Id { get; set; }


        [Required]
        public string Token { get; set; } = string.Empty; // *#* schema

        [Required]
        public string Ip { get; set; } = string.Empty; // *#* schema


        [Required]
        public string Phone { get; set; } = string.Empty; // *#* schema

        [Required]
        public string CountryCode { get; set; } = string.Empty; // *#* schema

        [Required]
        public string Password { get; set; } = string.Empty; // *#* schema


        [Required]
        public string Code { get; set; } = string.Empty;

        [Required]
        public int CodeCount { get; set; } = 1;

        [Required]
        public DateTime ExpiredDate { get; set; } = DateTime.UtcNow.AddMinutes(5);
    }

    public class RegTokenInformation
    {
        public string FirstName { get; set; } = string.Empty;
        public string MiddleName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;

        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        public DateTime ExpiredDate { get; set; } = DateTime.UtcNow.AddMinutes(ProgramSettings.TokenExpiredMinuts);
    }
}
