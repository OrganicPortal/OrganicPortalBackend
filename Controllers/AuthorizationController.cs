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
        public IAuthorization _authorizationService;
        public AuthorizationController(IAuthorization authorizationService)
        {
            _authorizationService = authorizationService;
        }


        [HttpPost("sign-in")]
        public async Task<IActionResult> PostSignInAsync(SignInIncomingObj incomingObj)
        {
            return (await _authorizationService.SignInAsync(incomingObj)).Results;
        }

        [HttpPost("sign-up")]
        public async Task<IActionResult> PostSignUpAsync(SignUpIncomingObj incomingObj)
        {
            return (await _authorizationService.SignUpAsync(incomingObj, getIp)).Results;
        }

        [HttpPost("sign-up/verif")]
        public async Task<IActionResult> PostVerifySignUpAsync(SignUpVerifIncomingObj incomingObj)
        {
            return (await _authorizationService.VerifySignUpAsync(incomingObj.Code, getRegToken, getIp)).Results;
        }

        [HttpGet("sign-up/retry")]
        public async Task<IActionResult> GetRetryVerifSMSAsync()
        {
            return (await _authorizationService.RetryVerifSMSAsync(getRegToken, getIp)).Results;
        }


        [HttpGet("refresh")]
        [Authorized]
        public async Task<IActionResult> GetRefreshAsync()
        {
            return Ok();
        }

        [HttpGet("get-roles")]
        [Authorized]
        public async Task<IActionResult> GetUserRolesAsync()
        {
            return (await _authorizationService.UserRoles(getRegToken)).Results;
        }


        [HttpPost("recovery/init")]
        public async Task<IActionResult> PostInitRecoveryAsync(InitRecoveryIncomingObj incomingObj)
        {
            return (await _authorizationService.InitRecoveryAsync(incomingObj)).Results;
        }

        [HttpPost("recovery/new-password")]
        public async Task<IActionResult> PostRecoveryAsync(RecoveryIncomingObj incomingObj)
        {
            return (await _authorizationService.RecoveryAsync(incomingObj, getRecoveryToken)).Results;
        }


        private string getIp { get { return "127.0.0.1"; } }
        private string getRegToken { get { return HttpContext.Request.Headers["RegToken"].FirstOrDefault() ?? ""; } }
        private string getRecoveryToken { get { return HttpContext.Request.Headers["RecoveryToken"].FirstOrDefault() ?? ""; } }
    }

    public class InitRecoveryIncomingObj
    {
        [Required]
        [Phone]
        public string Phone { get; set; } = string.Empty;
    }
    public class RecoveryIncomingObj
    {
        [Required]
        [Phone]
        public string Code { get; set; } = string.Empty;

        [Required]
        [RegularExpression(ProgramSettings.PasswordPattern)]
        public string Password { get; set; } = string.Empty;
    }

    public class SignInIncomingObj
    {
        [Required]
        [Phone]
        public string Login { get; set; } = string.Empty;

        [Required]
        [RegularExpression(ProgramSettings.PasswordPattern)]
        public string Password { get; set; } = string.Empty;
    }
    public class SignUpIncomingObj
    {
        [Required]
        [Length(2, 30)]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        [Length(2, 30)]
        public string MiddleName { get; set; } = string.Empty;

        [Required]
        [Length(2, 30)]
        public string LastName { get; set; } = string.Empty;

        [Required]
        [RegularExpression(ProgramSettings.PasswordPattern)]
        public string Password { get; set; } = string.Empty;

        [Required]
        [Phone]
        public string Phone { get; set; } = string.Empty;
    }
    public class SignUpVerifIncomingObj
    {
        [Required]
        [RegularExpression(ProgramSettings.CodePattern)]
        public string Code { get; set; } = string.Empty;
    }
}
