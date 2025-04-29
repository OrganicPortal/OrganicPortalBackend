using OrganicPortalBackend.Models.Database.Seed;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace OrganicPortalBackend.Models.Database.Company
{
    /*
     * Таблиця з інформацією про компанію
     * Інформація вноситься на 1 етапі, після створення кабінету. Може підтягуватися за ЄДРПОУ з єдиного державного реєстру
     * Перевірку коректності можна реалізувати через код на номер телефону, вказаний при реєстрації юр-особи
     * 
     * Вноситсья користувачем
     */
    public class CompanyModel
    {
        // Базова інформація
        /* */
        public long Id { get; set; }
        public DateTime CreatedDate { get; init; } = DateTime.UtcNow;
        /* */


        // Інформація про компанію
        /* */
        [Required]
        [MinLength(2)]
        // Ім'я установи (повна назва)
        // #exemple::ТМ Яскрава
        public string Name { get; set; } = string.Empty;

        [Required]
        [MinLength(2)]
        // Повний опис компанії (весь опис компанії для відображення в пошуку)
        // #exemple::Виробник якісного насіння, що захоплує всю Україну
        public string Description { get; set; } = string.Empty;

        [Required]
        [MinLength(2)]
        // Номер в державному реєстрі
        // #exemple::2115818089
        public string RegistrationNumber { get; set; } = string.Empty;

        [Required]
        [MinLength(2)]
        // Повна адреса до установи
        // #exemple::Україна, **0, Хмельницька обл., місто Хмельницький, ВУЛИЦЯ ПРИБУЗЬКА, будинок **, квартира **
        public string Address { get; set; } = string.Empty;

        [Required]
        // Рівень довіри
        // #exemple::Checked (Перевірений за реєстраційним номером)
        public EnumTrustStatus TrustStatus { get; set; } = EnumTrustStatus.OnInspection;

        [Required]
        // Тип власності організації
        // #exemple::SP (Фізична особа підприємець (ФОП))
        public EnumLegalType LegalType { get; set; } = EnumLegalType.Uncnown;

        [Required]
        // Дата заснування компанії
        // #exemple::2001 рік
        public DateTime EstablishmentDate { get; set; } = DateTime.UtcNow;

        [Required]
        // Чи заархівовано компанію
        // #exemple::false
        public bool isArchivated { get; set; } = false;

        // Дата архівування
        // #exemple::null
        public DateTime? ArchivationDate { get; set; } = null;
        /* */


        // Список зв'язків в таблицях бд
        /* */
        // Список контактної інформації
        public ICollection<CompanyContactModel> ContactsList { get; set; } = new List<CompanyContactModel>();

        // Список співробітників
        public ICollection<EmployeesModel> EmployeesList { get; set; } = new List<EmployeesModel>();

        // Список видів діяльності
        public ICollection<CompanyTypeOfActivityModel> TypeOfActivityList { get; set; } = new List<CompanyTypeOfActivityModel>();

        /* */
    }

    // Тип власності компанії
    public enum EnumLegalType
    {
        [Description("Не вказано")]
        Uncnown = 0,
        [Description("Фізична особа підприємець (ФОП)")]
        SP = 1,
        [Description("Товариство з обмеженою відповідальністю (ТОВ)")]
        LLC = 2
    }

    // Статус перевірки компанії
    public enum EnumTrustStatus
    {
        [Description("Невідомо")]
        Uncnown = 0,
        [Description("Перевіряється")]
        OnInspection = 1,
        [Description("Перевірений за реєстраційним номером")]
        Checked = 2
    }
}
