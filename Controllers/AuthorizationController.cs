using Microsoft.AspNetCore.Mvc;
using OrganicPortalBackend.Services;
using OrganicPortalBackend.Services.Attribute;
using System.ComponentModel.DataAnnotations;

namespace OrganicPortalBackend.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthorizationController : ControllerBase
    {
        // Ґетер отримання IP адреси користувача
        private string getIp { get { return HttpContext.Request.Headers["remote-ip-address"].FirstOrDefault() ?? string.Empty; } }
        // Ґетер отримання реєстраційного ідентифікатора запису
        private string getRegToken { get { return HttpContext.Request.Headers["RegToken"].FirstOrDefault() ?? ""; } }
        // Ґетер отримання ідентифікатора запиту відновлення паролю
        private string getRecoveryToken { get { return HttpContext.Request.Headers["RecoveryToken"].FirstOrDefault() ?? ""; } }
        // Ґетер отримання авторизаційного токену
        private string getToken { get { return HttpContext.Request.Headers["Authorization"].FirstOrDefault()?.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)[1] ?? ""; } }


        public IAuthorization _authorizationService;
        public AuthorizationController(IAuthorization authorizationService)
        {
            _authorizationService = authorizationService;
        }


        // Користувацькі ендпойнти
        /* */
        [HttpPost("sign-in")]
        // Ендпойнт входу в обліковий запис (Login)
        public async Task<IActionResult> PostSignInAsync(SignInIncomingObj incomingObj) => (await _authorizationService.SignInAsync(incomingObj)).Result;

        [HttpGet("sign-out")]
        // Ендпойнт реєстрації облікового запису (Registration)
        public async Task<IActionResult> GetSignOutAsync() => (await _authorizationService.SignOutAsync(getToken)).Result;

        [HttpPost("sign-up")]
        // Ендпойнт виходу з облікового запису (Logout)
        public async Task<IActionResult> PostSignUpAsync(SignUpIncomingObj incomingObj) => (await _authorizationService.SignUpAsync(incomingObj, getIp)).Result;

        [HttpPost("sign-up/verif")]
        // Ендпойнт підтвердження котактних даних, вказаних під час реєстрації
        public async Task<IActionResult> PostVerifySignUpAsync(SignUpVerifIncomingObj incomingObj) => (await _authorizationService.VerifySignUpAsync(incomingObj.Code, getRegToken, getIp)).Result;

        [HttpGet("sign-up/retry")]
        // Ендпойнт повторного надсилання коду підтвердження контактних даних при реєстрації
        public async Task<IActionResult> GetRetryVerifSMSAsync() => (await _authorizationService.RetryVerifSMSAsync(getRegToken, getIp)).Result;

        [HttpPost("recovery/init")]
        // Ендпойнт ініціації процесу відновлення доступу до облікового запису
        public async Task<IActionResult> PostInitRecoveryAsync(InitRecoveryIncomingObj incomingObj) => (await _authorizationService.InitRecoveryAsync(incomingObj)).Result;

        [HttpPost("recovery/new-password")]
        // Ендпойнт зміни паролю, за умови правильності коду відновлення
        public async Task<IActionResult> PostRecoveryAsync(RecoveryIncomingObj incomingObj) => (await _authorizationService.RecoveryAsync(incomingObj, getRecoveryToken)).Result;


        [Authorized]
        [HttpGet("get-roles")]
        // Ендпойнт отримання списку ролей авторизованого користувача
        public async Task<IActionResult> GetUserRolesAsync() => (await _authorizationService.UserRolesAsync(getToken)).Result;
        /* */
    }


    // Користувацькі вхідні об'єкти
    /* */
    // Вхідна інформація, для ініціації початку процесу відновлення паролю
    public class InitRecoveryIncomingObj
    {
        [Required]
        [Phone]
        // Контактна інформація
        // #exemple::+380765432134
        public string Phone { get; set; } = string.Empty;
    }

    // Вхідна інформація, для зміни паролю користувача у випадку правильності коду відновлення
    public class RecoveryIncomingObj
    {
        [Required]
        [RegularExpression(ProgramSettings.CodePattern)]
        // Код
        // #exemple::12345678
        public string Code { get; set; } = string.Empty;

        [Required]
        [RegularExpression(ProgramSettings.PasswordPattern)]
        // Новий пароль
        // #exemple::Qazwsx123$
        public string Password { get; set; } = string.Empty;
    }

    // Вхідна інформація, для здійснення входу до облікового запису
    public class SignInIncomingObj
    {
        [Required]
        [Phone]
        // Контактна інформація
        // #exemple::+380765432134
        public string Login { get; set; } = string.Empty;

        [Required]
        [RegularExpression(ProgramSettings.PasswordPattern)]
        // Пароль
        // #exemple::Qazwsx123$
        public string Password { get; set; } = string.Empty;
    }

    // Вхідна інформація, для реєстрації нового облікового запису
    public class SignUpIncomingObj
    {
        [Required]
        [Length(2, 30)]
        // Ім'я користувача
        // #exemple::Сергій
        public string FirstName { get; set; } = string.Empty;

        [Required]
        [Length(2, 30)]
        // По батькові користувача
        // #exemple::Вікторович
        public string MiddleName { get; set; } = string.Empty;

        [Required]
        [Length(2, 30)]
        // Прізвище користувача
        // #exemple::Глушков
        public string LastName { get; set; } = string.Empty;

        [Required]
        [RegularExpression(ProgramSettings.PasswordPattern)]
        // Пароль
        // #exemple::Qazwsx123$
        public string Password { get; set; } = string.Empty;

        [Required]
        [Phone]
        // Контактна інформація
        // #exemple::+380765432134
        public string Phone { get; set; } = string.Empty;
    }

    // Вхідна інформація, для підтвердження контактних даних, вказаних під час реєстрації
    public class SignUpVerifIncomingObj
    {
        [Required]
        [RegularExpression(ProgramSettings.CodePattern)]
        // Код
        // #exemple::12345678
        public string Code { get; set; } = string.Empty;
    }
    /* */
}
