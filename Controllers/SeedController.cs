using Microsoft.AspNetCore.Mvc;
using OrganicPortalBackend.Models;
using OrganicPortalBackend.Models.Database.Company;
using OrganicPortalBackend.Models.Database.Seed;
using OrganicPortalBackend.Services;
using OrganicPortalBackend.Services.Attribute;
using System.ComponentModel.DataAnnotations;

namespace OrganicPortalBackend.Controllers
{
    [Authorized]
    [Route("api/seeds")]
    [ApiController]
    public class SeedController : ControllerBase
    {
        public readonly ISeed _seedService;
        public SeedController(ISeed seedService)
        {
            _seedService = seedService;
        }


        // Користувацькі ендпойнти
        /* */
        [HttpPost("new")]
        [Roles(useCompanyId: true, roles: [EnumUserRole.Owner, EnumUserRole.Manager])]
        // Ендпойнт створення нового запису про насіння
        public async Task<IActionResult> PostNewSeedAsync([FromQuery] long companyId, SeedIncomingObj incomingObj) => (await _seedService.NewSeedAsync(companyId, incomingObj)).Result;

        [HttpPatch("edit")]
        [Roles(useCompanyId: true, roles: [EnumUserRole.Owner, EnumUserRole.Manager])]
        // Ендпойнт редагування інформації про насіння
        public async Task<IActionResult> PatchEditSeedAsync([FromQuery] long seedId, [FromQuery] long companyId, SeedIncomingObj incomingObj) => (await _seedService.EditSeedAsync(seedId, companyId, incomingObj)).Result;

        [HttpGet("info")]
        [Roles(useCompanyId: true, roles: [EnumUserRole.Owner, EnumUserRole.Manager])]
        // Ендпойнт отримання інформації про вказане насіння
        public async Task<IActionResult> GetSeedInfoAsync([FromQuery] long seedId, [FromQuery] long companyId) => (await _seedService.SeedInfoAsync(seedId, companyId)).Result;

        [HttpDelete("remove")]
        [Roles(useCompanyId: true, roles: [EnumUserRole.Owner, EnumUserRole.Manager])]
        // Ендпойнт видалення запису про насіння
        public async Task<IActionResult> DeleteRemoveSeedAsync([FromQuery] long seedId, [FromQuery] long companyId) => (await _seedService.RemoveSeedAsync(seedId, companyId)).Result;

        [HttpPost("list")]
        [Roles(useCompanyId: true, roles: [EnumUserRole.Owner, EnumUserRole.Manager])]
        // Ендпойнт отримання списку насіння компанії
        public async Task<IActionResult> PostSeedListAsync([FromQuery] long companyId, Paginator paginator) => (await _seedService.SeedListAsync(companyId, paginator)).Result;


        [HttpGet("send-certifications")]
        [Roles(useCompanyId: true, roles: [EnumUserRole.Owner, EnumUserRole.Manager])]
        // Ендпойнт відправки запиту для початку процесу сертифікації насіння
        public async Task<IActionResult> GetSendSeedToCertificationAsync([FromQuery] long seedId, [FromQuery] long companyId) => (await _seedService.SendSeedToCertificationAsync(seedId, companyId)).Result;

        [HttpPost("certs/add")]
        [Roles(useCompanyId: true, roles: [EnumUserRole.Owner, EnumUserRole.Manager])]
        // Ендпойнт додавання сертифікату до списку сертифікатів насіння
        public async Task<IActionResult> PostAddCERTAsync([FromQuery] long seedId, [FromQuery] long companyId, CERTIncomingObj incomingObj) => (await _seedService.AddCERTAsync(seedId, companyId, incomingObj)).Result;

        [HttpDelete("certs/remove")]
        [Roles(useCompanyId: true, roles: [EnumUserRole.Owner, EnumUserRole.Manager])]
        // Ендпойнт видалення сертифікату зі списку сертифікатів насіння
        public async Task<IActionResult> DeleteRemoveCERTAsync([FromQuery] long UseCERTId, [FromQuery] long companyId) => (await _seedService.RemoveCERTAsync(UseCERTId, companyId)).Result;

        [HttpGet("certs")]
        // Ендпойнт отримання списку сертифікатів для насіння
        public async Task<IActionResult> GetCERTList() => (await _seedService.CERTList()).Result;
        /* */
    }


    // Користувацькі вхідні об'єкти
    /* */
    // Вхідна інформація, для формування запсиу про насіння
    public class SeedIncomingObj
    {
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
    }

    // Вхідна інформація, для формування сертифікатів
    public class CERTIncomingObj
    {
        [Required]
        [Range(1, long.MaxValue)]
        // Ідентифікатор сертифікату
        public long CERTId { get; set; }
    }
    /* */
}
