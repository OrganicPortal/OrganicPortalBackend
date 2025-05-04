using OrganicPortalBackend.Models.Database.Seed.CERT;
using System.ComponentModel.DataAnnotations;

namespace OrganicPortalBackend.Models.Database.Seed
{
    /*
     * Таблиця прив'язки інформації про сертифікат до запису про насіння
     * Інформація прикріпюється користувачем як доповнення
     * Вона доповнює загальний опис насіння, та дозволяє підвищити рівень довіри до насіння
     * 
     * Вноситься користувачем
     */
    public class UseCERTModel
    {
        // Базова інформація
        /* */
        [Key]
        public long Id { get; set; }
        public DateTime CreatedDate { get; init; } = DateTime.UtcNow;
        /* */


        // Інформація про перевірку сертифікату
        /* */
        // Чи був запис верифікований лабораторією
        public bool IsVerified { get; set; } = false;
        /* */


        // Список зв'язків в таблицях бд
        /* */
        [Required]
        [Range(1, long.MaxValue)]
        // Ідентифікатор насіння
        public long SeedId { get; set; }
        public SeedModel? Seed { get; set; } = null;

        [Required]
        [Range(1, long.MaxValue)]
        // Ідентифікатор сертифікату
        public long CERTId { get; set; }
        public CERTModel? CERT { get; set; } = null;

        // Доповнена інформація з лабораторного висновку
        public long? CERTAdditionalId { get; set; }
        public CERTAdditionalModel? CERTAdditional { get; set; } = null;

        // Фото підтвердження сертифікату
        public ICollection<CERTFileModel> FilesList { get; set; } = new List<CERTFileModel>();
        /* */
    }
}
