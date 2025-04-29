using Microsoft.AspNetCore.Mvc;
using OrganicPortalBackend.Models.Database.Company;
using OrganicPortalBackend.Services;
using OrganicPortalBackend.Services.Attribute;
using System.ComponentModel.DataAnnotations;

namespace OrganicPortalBackend.Controllers
{
    [ApiController]
    [Authorized]
    [Route("api/company")]
    public class CompanyController : ControllerBase
    {
        private string getToken { get { return HttpContext.Request.Headers["Authorization"].FirstOrDefault()?.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)[1] ?? ""; } }

        public readonly ICompany _companyService;
        public CompanyController(ICompany companyService)
        {
            _companyService = companyService;
        }


        [HttpPost("new")]
        public async Task<IActionResult> PostNewCompanyAsync(CompanyIncomingObj incomingObj)
        {
            return (await _companyService.NewCompanyAsync(incomingObj, getToken)).Results;
        }

        [HttpPatch("edit")]
        [Roles(useCompanyId: true, roles: [EnumUserRole.Owner])]
        public async Task<IActionResult> PatchEditCompanyAsync(EditCompanyIncomingObj incomingObj)
        {
            return (await _companyService.EditCompanyAsync(incomingObj)).Results;
        }

        [HttpGet("my-company")]
        public async Task<IActionResult> GetMyCompanyAsync()
        {
            return (await _companyService.MyCompanyAsync(getToken)).Results;
        }

        [HttpGet("info")]
        [Roles(useCompanyId: true, roles: [EnumUserRole.Owner])]
        public async Task<IActionResult> GetCompanyInfo([FromQuery] long companyId)
        {
            return (await _companyService.CompanyInfoAsync(companyId)).Results;
        }

        [HttpGet("archiving")]
        [Roles(useCompanyId: true, roles: [EnumUserRole.Owner, EnumUserRole.Manager])]
        public async Task<IActionResult> PostArchivateCompanyAsync([FromQuery] long companyId)
        {
            return (await _companyService.ArchivateCompanyAsync(companyId)).Results;
        }
    }

    // Вхідна інформація, для редагування даних компанії
    public class EditCompanyIncomingObj
    {
        [Required]
        [Range(1, long.MaxValue)]
        // Ідентифікатор компанії
        public long CompanyId { get; set; }

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
        public ICollection<EnumTypeOfInteractivity> TypeOfInteractivityList { get; set; } = new List<EnumTypeOfInteractivity>();
    }

    // Вхідна інформація, про контактні дані компанії
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
}
