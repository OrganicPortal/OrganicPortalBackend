using Microsoft.AspNetCore.Mvc;
using OrganicPortalBackend.Services;
using OrganicPortalBackend.Services.Attribute;
using System.ComponentModel.DataAnnotations;

namespace OrganicPortalBackend.Controllers
{
    [Route("api/users")]
    [Authorized]
    [ApiController]
    public class UserController : ControllerBase
    {
        private string getToken { get { return HttpContext.Request.Headers["Authorization"].FirstOrDefault()?.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)[1] ?? ""; } }

        public readonly IUser _userService;
        public UserController(IUser userService)
        {
            _userService = userService;
        }

        // Користувацькі ендпойнти
        /* */
        [HttpGet("my-profiles")]
        // Дані профілю користувача
        public async Task<IActionResult> GetMyCompanyAsync()
        {
            return (await _userService.MyProfileAsync(getToken)).Result;
        }

        [HttpPatch("my-profiles/edits")]
        // Редагуванні інформації профілю
        public async Task<IActionResult> PatchMyProfileAsync(EditUserProfileIncomingObj incomingObj)
        {
            return (await _userService.EditMyProfileAsync(incomingObj, getToken)).Result;
        }
        /* */
    }


    // Користувацькі вхідні об'єкти
    /* */
    // Вхідна інформація по редагуванню профілю
    public class EditUserProfileIncomingObj
    {
        [Required]
        [Range(1, long.MaxValue)]
        // #exemple::1
        // Ідентифікатор користувача
        public long UserId { get; set; }

        [Required]
        [Length(2, 30)]
        // Ідентифікатор користувача
        // #exemple::Сергій
        public string FirstName { get; set; } = string.Empty;

        [Required]
        [Length(2, 30)]
        // Ідентифікатор користувача
        // #exemple::Вікторович
        public string MiddleName { get; set; } = string.Empty;

        [Required]
        [Length(2, 30)]
        // Ідентифікатор користувача
        // #exemple::Глушков
        public string LastName { get; set; } = string.Empty;
    }
    /* */
}
