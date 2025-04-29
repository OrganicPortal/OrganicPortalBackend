using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace OrganicPortalBackend.Models.Database.Company
{
    /*
     * Таблиця з інформацією про вид діяльності
     * Інформація вноситься на 1 етапі, після створення кабінету. Може підтягуватися за ЄДРПОУ з єдиного державного реєстру
     * Список діяльності користувача (можна вважати тегами)
     * 
     * Вноситсья користувачем
     */
    public class CompanyTypeOfActivityModel
    {
        // Базова інформація
        /* */
        public long Id { get; set; }
        public DateTime CreatedDate { get; init; } = DateTime.UtcNow;
        /* */


        // Інформація про тип діяльності
        /* */
        [Required]
        // Вид діяльності
        // #exemple::SeedProduction (Виробництво та пакування насіння)
        public EnumTypeOfInteractivity Type { get; set; } = EnumTypeOfInteractivity.SeedProduction;
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

    // Список видів діяльності
    public enum EnumTypeOfInteractivity
    {
        [Description("Виробництво та пакування насіння")]
        SeedProduction = 0,
        [Description("Дистрибуція та оптовий продаж насіння")]
        SeedDistribution = 1,
        [Description("Роздрібна торгівля насінням, добривами, засобами захисту рослин (ЗЗР) тощо")]
        RetailSales = 2,
        [Description("Селекція та виведення нових сортів")]
        PlantBreeding = 3,
        [Description("Імпорт та експорт насіння або супутніх товарів")]
        ImportExport = 4
    }
}
