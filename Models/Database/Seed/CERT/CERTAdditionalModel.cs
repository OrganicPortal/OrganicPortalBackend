using System.ComponentModel.DataAnnotations;

namespace OrganicPortalBackend.Models.Database.Seed.CERT
{
    /*
     * Таблиця з доповненою інформацією про насіння
     * Інформація вноситься згідно виданого сертифікату
     * Ця інформація доповнює загальний опис і додається до смарт-контракту в якості доповнення
     * 
     * Вноситься користувачем/лабораторією (в процесі)
     * ДСТУ 7160:2020
     */
    public class CERTAdditionalModel
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
        [Range(-1, float.MaxValue)]
        // Вказує на відсоток сходжень
        // #exemple::95%
        public float GerminationRate { get; set; } = -1;

        [Required]
        [Range(-1, float.MaxValue)]
        // Потужність проростання на ранніх етапах
        // #exemple::95%
        public float EnergyOfGermination { get; set; } = -1;

        [Required]
        [Range(-1, float.MaxValue)]
        // Відсоток заявлених рослин від загальної кількості
        // #exemple::100%
        public float PurityPercentage { get; set; } = -1;

        [Required]
        [Range(-1, float.MaxValue)]
        // Відсоток вологи в насінні (8-14% дозволених)
        // #exemple::9%
        public float MoistureContent { get; set; } = -1;

        [Required]
        [Range(-1, float.MaxValue)]
        // Відсоток аномальних рослин (дефектних, слабких, тощо)
        // #exemple::9%
        public float AbnormalSeedlingsPercentage { get; set; } = -1;

        [Required]
        [Range(-1, float.MaxValue)]
        // Частина насіння, яка проростає не відразу
        // #exemple::0%
        public float HardSeedsPercentage { get; set; } = -1;

        [Required]
        [Range(-1, float.MaxValue)]
        // Відсоток бур'янів
        // #exemple::0%
        public float WeedSeedsPercentage { get; set; } = -1;

        [Required]
        [Range(-1, float.MaxValue)]
        // Рівень інертного забруднення насіння (пісок, земля, пил тощо)
        // #exemple::0%
        public float InertMatterPercentage { get; set; } = -1;

        // Додаткові відомості
        // #exemple::
        public string Note { get; set; } = string.Empty;

        // Дата початку лабораторної перевірки
        public DateTime IssueDate { get; set; } = DateTime.UtcNow;

        // Дата закінчення лабораторної перевірки
        public DateTime? ExpiryDate { get; set; } = null;
        /* */
    }
}
