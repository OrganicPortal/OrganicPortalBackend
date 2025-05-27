using Microsoft.AspNetCore.Mvc;
using OrganicPortalBackend.Services;
using OrganicPortalBackend.Services.Attribute;
using System.ComponentModel.DataAnnotations;

namespace OrganicPortalBackend.Controllers
{
    [Authorized]
    [Route("api/users")]
    [ApiController]
    public class UserController : ControllerBase
    {
        // Ґетер отримання авторизаційного токену
        private string getToken { get { return HttpContext.Request.Headers["Authorization"].FirstOrDefault()?.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)[1] ?? ""; } }


        public readonly IUser _userService;
        public UserController(IUser userService)
        {
            _userService = userService;
        }


        // Користувацькі ендпойнти
        /* */
        [HttpGet("my-profile")]
        // Ендпойнт отримання даних профілю авторизованого користувача
        public async Task<IActionResult> GetMyProfileAsync() => (await _userService.MyProfileAsync(getToken)).Result;

        [HttpPatch("my-profiles/edits")]
        // Ендпойнт редагування профілю авторизованого користувача
        public async Task<IActionResult> PatchEditMyProfileAsync(EditUserProfileIncomingObj incomingObj) => (await _userService.EditMyProfileAsync(incomingObj, getToken)).Result;
        /* */
    }


    // Користувацькі вхідні об'єкти
    /* */
    // Вхідна інформація, для редагування профілю
    public class EditUserProfileIncomingObj
    {
        [Required]
        [Range(1, long.MaxValue)]
        // #exemple::1
        // Ідентифікатор користувача
        public long UserId { get; set; }

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
    }
    /* */
}
