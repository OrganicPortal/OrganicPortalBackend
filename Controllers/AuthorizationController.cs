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


        private string getIp { get { return "127.0.0.1"; } }
        private string getRegToken { get { return HttpContext.Request.Headers["RegToken"].FirstOrDefault() ?? ""; } }
        private string getToken { get { return HttpContext.Request.Headers["Authorization"].FirstOrDefault()?.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)[1] ?? ""; } }
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
