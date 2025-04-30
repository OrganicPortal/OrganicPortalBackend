using System.ComponentModel.DataAnnotations;

namespace OrganicPortalBackend.Models.Database.Seed.CERT
{
    /*
     * Таблиця з фото сертифікату
     * Інформація вноситсья користувачем і 
     * Ця інформація доповнює загальний опис і додається до смарт-контракту в якості доповнення
     * 
     * Вноситься користувачем
     */
    public class CERTFileModel
    {
        // Базова інформація
        /* */
        [Key]
        public long Id { get; set; }
        public DateTime CreatedDate { get; init; } = DateTime.UtcNow;
        /* */


        // Інформація про сертифікат
        /* */
        [Required]
        [MinLength(2)]
        // Назва файлу
        // #exemple::Сертифікато схожості.pdf
        public string Name { get; set; } = string.Empty;

        [Required]
        [MinLength(2)]
        // Покликання на файл
        // #exemple::/cert-name/102032/Сертифікато схожості.pdf
        public string Href { get; set; } = string.Empty;
        /* */


        // Список зв'язків в таблицях бд
        /* */
        [Required]
        [Range(1, long.MaxValue)]
        // Сертифікат
        public long UseCERTId { get; set; }
        public UseCERTModel? UseCERT { get; set; } = null;
        /* */
    }
}
