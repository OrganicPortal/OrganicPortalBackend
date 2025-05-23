﻿using Microsoft.AspNetCore.Mvc;
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
        public async Task<IActionResult> PostSignInAsync(SignInIncomingObj incomingObj) => (await _authorizationService.SignInAsync(incomingObj)).Result;

        [HttpGet("sign-out")]
        public async Task<IActionResult> GetSignOutAsync() => (await _authorizationService.SignOutAsync(getToken)).Result;

        [HttpPost("sign-up")]
        public async Task<IActionResult> PostSignUpAsync(SignUpIncomingObj incomingObj) => (await _authorizationService.SignUpAsync(incomingObj, getIp)).Result;

        [HttpPost("sign-up/verif")]
        public async Task<IActionResult> PostVerifySignUpAsync(SignUpVerifIncomingObj incomingObj) => (await _authorizationService.VerifySignUpAsync(incomingObj.Code, getRegToken, getIp)).Result;

        [HttpGet("sign-up/retry")]
        public async Task<IActionResult> GetRetryVerifSMSAsync() => (await _authorizationService.RetryVerifSMSAsync(getRegToken, getIp)).Result;

        [HttpGet("get-roles")]
        [Authorized]
        public async Task<IActionResult> GetUserRolesAsync() => (await _authorizationService.UserRoles(getToken)).Result;


        [HttpPost("recovery/init")]
        public async Task<IActionResult> PostInitRecoveryAsync(InitRecoveryIncomingObj incomingObj) => (await _authorizationService.InitRecoveryAsync(incomingObj)).Result;

        [HttpPost("recovery/new-password")]
        public async Task<IActionResult> PostRecoveryAsync(RecoveryIncomingObj incomingObj) => (await _authorizationService.RecoveryAsync(incomingObj, getRecoveryToken)).Result;


        private string getIp { get { return HttpContext.Request.Headers["remote-ip-address"].FirstOrDefault() ?? string.Empty; } }
        private string getRegToken { get { return HttpContext.Request.Headers["RegToken"].FirstOrDefault() ?? ""; } }
        private string getRecoveryToken { get { return HttpContext.Request.Headers["RecoveryToken"].FirstOrDefault() ?? ""; } }
        private string getToken { get { return HttpContext.Request.Headers["Authorization"].FirstOrDefault()?.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)[1] ?? ""; } }
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
