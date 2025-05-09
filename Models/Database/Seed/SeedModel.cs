using OrganicPortalBackend.Models.Database.Company;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace OrganicPortalBackend.Models.Database.Seed
{
    /*
     * Процес внесення і верифікації інформації про насіння
     * 1) Користувач заповнює інформацію про насіння
     *  1.1) Заповнює дані (SeedModel)
     *  1.2) Підв'язує підтримувані сертифікати (UseCERTModel)
     *  1.3) Перевріє дані та надсилає їх на затвердження до сервісу Organic portal (система блокує можливість редагування запису користувачу)
     * 
     * 2) Система оцінює надіслану інформацію
     *  2.1) Адміністратор сертифікату (він є обов'язковим) приймає заявку
     *  2.2) Проводиться лаборараторна перевірка
     *  2.3) Видаєтсья сертифікат
     * 
     * 3) Система проводить верифікацію насіння
     *  3.1) Приймається інформація про насіння і сертифікат
     *  3.2) Генеруєтсья токен (хронологічний ідентифікатор запису)
     *  3.3) Вся необхідна інформація надсилається сервісу Solana для підписання (створюєтсья смарт контракт)
     * 
     * 4) В системі відображаєтсья верифіковане насіння з посиланням на Solana 
     *      Все що стосуєтсья попередніх етапів відображається в Solana
     *      Формуєтсья QR код для нанесення його на товар
     *      Формуєтсья сторінка насіння в системі Organic Portal
     *      Насіння стає доступним для використання фермерами (наступного етапу верифікації)
     *      
     * Результуючим виглядом є підписані дані, з якими продовжується робота. Редагування цих даних неможливе, доведетсья перевипускати токен повторно
     */










    /*
     * Таблиця з інформацією про насіння
     * Інформація вноситсья користувачем (людиною що виробила насіння). Заповнюються поля у чіткій відповідності до форми.
     * Ця інформація є сирою і затверджуєтсья верифікатором (лабораторією). Після підтвердження лабораторією, всі інформація верифікується сервісом Solana і вноситься в таблицю, без можливості змін
     * 
     * Вноситься користувачем
     */
    public class SeedModel
    {
        // Базова інформація
        /* */
        [Key]
        public long Id { get; set; }
        public DateTime CreatedDate { get; init; } = DateTime.UtcNow;

        // Ключ історії записів
        public string HistoryKey { get; set; } = string.Empty;
        /* */


        // Інформація про насіння
        /* */
        // https://yaskrava.com.ua/ua/semena/semena-ovoschey-pakety-giganty/kukurudza-paket-gigant/kukuruza-medunka-f1-20gr
        [Required]
        [MinLength(2)]
        // Узагальнене ім'я культури
        // #exemple::Кукурудза цукрова
        public string Name { get; set; } = string.Empty;

        [Required]
        [MinLength(2)]
        // Узагальнене ім'я культури на латині
        // #exemple::Zea mays saccharata Sturt
        public string ScientificName { get; set; } = string.Empty;

        [Required]
        [MinLength(2)]
        // Сорт рослини
        // #exemple::Медунка F1
        public string Variety { get; set; } = string.Empty;

        [Required]
        [MinLength(2)]
        // Тип насіння (органічне, гібридне тощо)
        // #exemple::Гібрид F1
        public string SeedType { get; set; } = string.Empty;

        [Required]
        [MinLength(2)]
        // Номер партії
        // #exemple::4823069912888
        public string BatchNumber { get; set; } = string.Empty;

        [Required]
        [MinLength(2)]
        // Дата виготовлення (збирання)
        // #exemple::10.03.2025
        public DateTime HarvestDate { get; set; } = DateTime.UtcNow;

        [Required]
        // Термін придатності
        // #exemple::10.10.2029
        public DateTime ExpiryDate { get; set; } = DateTime.UtcNow;

        [Required]
        // Оброблене насіння
        // #exemple::Untreated (Не оброблене)
        public EnumTreatmentType TreatmentType { get; set; } = EnumTreatmentType.Uncnown;

        [Required]
        [MinLength(2)]
        // Умови зберігання насіння
        // #exemple::В сухому місці, при температурах від 6°-40° градусів
        public string StorageConditions { get; set; } = string.Empty;

        [Required]
        [Range(0, double.MaxValue)]
        // Вага 1 тисячі насіннин в грамах
        // #exemple::~1176,47 г
        public double AverageWeightThousandSeeds { get; set; } = 0;

        [Required]
        // Новий запис видимий тільки користувачу
        // #exemple::New (Новий запис)
        public EnumSeedStatus Status { get; set; } = EnumSeedStatus.New;
        /* */


        // Список зв'язків в таблицях бд
        /* */
        [Required]
        [Range(1, long.MaxValue)]
        // Ідентифікатор компанії
        public long CompanyId { get; set; }
        public CompanyModel? Company { get; set; } = null;

        // Список сертифікатів
        public ICollection<UseCERTModel> CERTsList { get; set; } = new List<UseCERTModel>(); // Список сертифікатів
        /* */
    }

    // Тип обробки насіння
    public enum EnumTreatmentType
    {
        [Description("Не вказано")]
        Uncnown = 0,
        [Description("Не оброблене")]
        Untreated = 1,
        [Description("Оброблене")]
        Treated = 2,
    }

    // Статус обробки подання про насіння
    public enum EnumSeedStatus
    {
        [Description("Новий запис")]
        New = 0,
        [Description("В очікуванні підтвердження сертифікатів")]
        AwaitCertificateConfirmation = 1,
        [Description("Очікує підписання")]
        AwaiSigning = 2,
        [Description("Підписане")]
        Signed = 3,
    }







    // В процесі
    public enum EnumEntry
    {
        [Description("Затверджений запис")]
        Confirmation, // Підписаний Solan-ою

        [Description("Заархівований підписаний запис")]
        Archivated, // Заархівований запис
    }
}