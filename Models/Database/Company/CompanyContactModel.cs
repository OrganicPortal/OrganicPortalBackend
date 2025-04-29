using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace OrganicPortalBackend.Models.Database.Company
{
    /*
     * Таблиця з контактною інформацією про компанію
     * Інформація з контактними даними компанії
     * Це моужуть бути сайти, телефонні номери, адреси пошти, соціальні мережі, тощо
     * 
     * Вноситсья користувачем
     */
    public class CompanyContactModel
    {
        // Базова інформація
        /* */
        public long Id { get; set; }
        public DateTime CreatedDate { get; init; } = DateTime.UtcNow;
        /* */


        // Контактна інформація
        /* */
        [Required]
        // Тип контактної інформації
        // #exemple::Phone (Номер телефону)
        public EnumCompanyContactType Type { get; set; } = EnumCompanyContactType.Uncnown;

        [Required]
        [MinLength(2)]
        // Контактна інформація
        // #exemple::+380765432134
        public string Contact { get; set; } = string.Empty;
        /* */


        // Список зв'язків в таблицях бд
        /* */
        [Required]
        [Range(1, long.MaxValue)]
        // Ідентифікатор компанії
        public long CompanyId { get; set; }
        public CompanyModel? Company { get; set; } = null;
        /* */
    }

    // Доступні типи контактної інформації
    public enum EnumCompanyContactType
    {
        [Description("Не відомо")]
        Uncnown = 0,
        [Description("Номер телефону")]
        Phone = 1,
    }
}
