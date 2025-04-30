using System.ComponentModel.DataAnnotations;

namespace OrganicPortalBackend.Models.Database.Seed.CERT
{
    /*
     * Таблиця з доступними для вибору сертифікатами
     * Це список стандартних, існуючих сертифікатів, що підтримуватимуться
     * Поки може бути лише сертифікат "Organic, Base. EU/UA release 2025"
     * 
     * Вноситься сервісом
     */
    public class CERTModel
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
        // Назва сертифікату
        // #exemple::Organic, Base EU/UA from 2025 year, issue I
        public string Name { get; set; } = string.Empty;

        [Required]
        [MinLength(2)]
        // Номер сертифікату
        // #exemple::OB-EU/UA-2025-I (Organic, Base)
        public string Number { get; set; } = string.Empty;

        [Required]
        [MinLength(2)]
        // Надавач послуг сертифікації
        // #exemple::Портал органічної рослинності (Organic Portal)
        public string IssuedBy { get; set; } = string.Empty;

        [Required]
        [MinLength(2)]
        // Опис сертифікату
        // #exemple::Базовий сертифікат якості насіння в системі OrganicPortal. Насіння не є перевіреним лабораторією "Organic Portal"
        public string Description { get; set; } = string.Empty;

        [Required]
        // Чи повинен містити додаткову інформацію
        // #exemple::false
        public bool IsAddlInfo { get; set; } = false;
        /* */
    }
}
