using Microsoft.AspNetCore.Mvc;
using OrganicPortalBackend.Models;
using OrganicPortalBackend.Models.Database.Company;
using OrganicPortalBackend.Services;
using OrganicPortalBackend.Services.Attribute;
using System.ComponentModel.DataAnnotations;

namespace OrganicPortalBackend.Controllers
{
    [Authorized]
    [Route("api/companies")]
    [ApiController]
    public class CompanyController : ControllerBase
    {
        // Ґетер отримання авторизаційного токену
        private string getToken { get { return HttpContext.Request.Headers["Authorization"].FirstOrDefault()?.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)[1] ?? ""; } }


        public readonly ICompany _companyService;
        public CompanyController(ICompany companyService)
        {
            _companyService = companyService;
        }


        // Користувацькі ендпойнти
        /* */
        [HttpPost("new")]
        // Ендпойнт створення нової компанії
        public async Task<IActionResult> PostNewCompanyAsync(CompanyIncomingObj incomingObj) => (await _companyService.NewCompanyAsync(incomingObj, getToken)).Result;

        [HttpPatch("edit")]
        [Roles(useCompanyId: true, roles: [EnumUserRole.Owner])]
        // Ендпойнт редагування інформації про компанію
        public async Task<IActionResult> PatchEditCompanyAsync([FromQuery] long companyId, EditCompanyIncomingObj incomingObj) => (await _companyService.EditCompanyAsync(companyId, incomingObj)).Result;

        [HttpGet("my-companies")]
        // Ендпойнт отримання списку всіх компаній авторизованого користувача
        public async Task<IActionResult> GetMyCompanyAsync() => (await _companyService.MyCompanyAsync(getToken)).Result;

        [HttpGet("info")]
        [Roles(useCompanyId: true, roles: [EnumUserRole.Owner])]
        // Ендпойнт отримання інформації про компанію
        public async Task<IActionResult> GetCompanyInfoAsync([FromQuery] long companyId) => (await _companyService.CompanyInfoAsync(companyId)).Result;


        [HttpGet("archiving")]
        [Roles(useCompanyId: true, roles: [EnumUserRole.Owner, EnumUserRole.Manager])]
        // Ендпойнт архівування компанії (аналог видалення)
        public async Task<IActionResult> GetArchivateCompanyAsync([FromQuery] long companyId) => (await _companyService.ArchivateCompanyAsync(companyId)).Result;

        [HttpGet("is-archivated")]
        [Roles(useCompanyId: true, roles: [EnumUserRole.Owner, EnumUserRole.Manager])]
        // Ендпойнт отримання статусу компанії (чи архівована чи ні)
        public async Task<IActionResult> GetCheckArchivatedCompanyAsync([FromQuery] long companyId) => (await _companyService.CheckArchivatedCompanyAsync(companyId)).Result;
        /* */


        // Адміністративні ендпойнти
        /* */
        [HttpPost("list")]
        [Roles(useCompanyId: false, roles: [EnumUserRole.SysAdmin, EnumUserRole.SysManager])]
        // Ендпойнт отримання списку компаній системи
        public async Task<IActionResult> PostCompanyListAsync(ListIncomingObj incomingObj) => (await _companyService.CompanyListAsync(incomingObj.Paginator)).Result;

        [HttpGet("")]
        [Roles(useCompanyId: false, roles: [EnumUserRole.SysAdmin, EnumUserRole.SysManager])]
        // Ендпойнт отримання інформації про компанію
        public async Task<IActionResult> GetCompanyAsync([FromQuery] long companyId) => (await _companyService.CompanyAsync(companyId)).Result;

        [HttpPatch("set-trust")]
        [Roles(useCompanyId: false, roles: [EnumUserRole.SysAdmin, EnumUserRole.SysManager])]
        // Ендпойнт редагування рівня довіри до компанії в системі OrganicPortal
        public async Task<IActionResult> PatchChangeTrustCompanyAsync([FromQuery] long companyId, [FromQuery] EnumTrustStatus trustStatus) => (await _companyService.ChangeTrustCompanyAsync(companyId, trustStatus)).Result;
        /* */
    }


    // Користувацькі вхідні об'єкти
    /* */
    // Вхідна інформація, для редагування даних компанії
    public class EditCompanyIncomingObj
    {
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
        // Повна адреса до установи
        // #exemple::Україна, **0, Хмельницька обл., місто Хмельницький, ВУЛИЦЯ ПРИБУЗЬКА, будинок **, квартира **
        public string Address { get; set; } = string.Empty;

        [Required]
        // Тип власності організації
        // #exemple::SP (Фізична особа підприємець (ФОП))
        public EnumLegalType LegalType { get; set; } = EnumLegalType.Uncnown;

        [Required]
        // Дата заснування компанії
        // #exemple::2001 рік
        public DateTime EstablishmentDate { get; set; } = DateTime.UtcNow;


        [Required]
        [MinLength(1)]
        // Контактні дані компанії
        public ICollection<CompanyContactIncomingObj> ContactList { get; set; } = new List<CompanyContactIncomingObj>();

        // Види діяльності
        public ICollection<int> TypeOfActivityList { get; set; } = new List<int>();
    }

    // Вхідна інформація, для формування нової компанії
    public class CompanyIncomingObj
    {
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
        // Тип власності організації
        // #exemple::SP (Фізична особа підприємець (ФОП))
        public EnumLegalType LegalType { get; set; } = EnumLegalType.Uncnown;

        [Required]
        // Дата заснування компанії
        // #exemple::2001 рік
        public DateTime EstablishmentDate { get; set; } = DateTime.UtcNow;


        [Required]
        [MinLength(1)]
        // Контактні дані компанії
        public ICollection<CompanyContactIncomingObj> ContactList { get; set; } = new List<CompanyContactIncomingObj>();

        // Види діяльності
        public ICollection<int> TypeOfActivityList { get; set; } = new List<int>();
    }

    // Вхідна інформація, для контактних даних компанії
    public class CompanyContactIncomingObj
    {
        [Required]
        [AllowedValues(EnumCompanyContactType.Phone)]
        // Тип контактної інформації
        // #exemple::Phone (Номер телефону)
        public EnumCompanyContactType Type { get; set; } = EnumCompanyContactType.Uncnown;

        [Required]
        [MinLength(2)]
        // Контактна інформація
        // #exemple::+380765432134
        public string Contact { get; set; } = string.Empty;
    }
    /* */


    // Адміністративні вхідні об'єкти
    /* */
    // Вхідна інформація, для отримання списку компаній системи
    public class ListIncomingObj
    {
        [Required]
        // Пагінатор стоірнок
        public Paginator Paginator { get; set; }
    }
    /* */
}
