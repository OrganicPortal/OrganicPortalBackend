using Microsoft.AspNetCore.Mvc;
using OrganicPortalBackend.Models;
using OrganicPortalBackend.Services;
using System.ComponentModel.DataAnnotations;

namespace OrganicPortalBackend.Controllers
{
    [Route("api/signed-info")]
    [ApiController]
    public class SolanaCERTController : ControllerBase
    {
        public readonly ISolanaCERT _solanaCERT;
        public SolanaCERTController(ISolanaCERT solanaCERT)
        {
            _solanaCERT = solanaCERT;
        }


        // Користувацькі ендпойнти
        /* */
        //[HttpGet("confirmations")]
        //// Надсилає запит на формування довіреного запису про насіння
        //public async Task<IActionResult> SolanaConfirmation([FromQuery] long seedId, [FromQuery] long companyId) => (await _solanaCERT.SendSeedToSolanaAsync(seedId)).Result;

        [HttpPost("reads")]
        // Ендпойнт на отримання інформації про насіння
        public async Task<IActionResult> SolanaReads(ReadIncomingObj incomingObj) => (await _solanaCERT.SolanaReadsAsync(incomingObj.PubKey)).Result;

        [HttpPost("list")]
        // Ендпойнт на отримання верифікованого списку з насінням
        public async Task<IActionResult> PostSolanaSeeds(SolanaSeedListIncomingObj incomingObj) => (await _solanaCERT.GetSolanaSeeds(incomingObj)).Result;
        /* */
    }

    // Користувацькі вхідні об'єкти
    /* */
    // Об'єкт на читання інформації про насіння
    public class ReadIncomingObj
    {
        public string PubKey { get; set; } = string.Empty;
    }

    // Об'єкт на отримання спсику насіння
    public class SolanaSeedListIncomingObj
    {
        [Required]
        // Пагінарор записів
        public Paginator Paginator { get; set; } = new Paginator();

        [AllowedValues(-1, 0, 1, 2)]
        // Оброблене чи не оброблене насіння
        public int TreatmentType { get; set; } = -1;
    }
    /* */
}
