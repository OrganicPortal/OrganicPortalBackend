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
        [HttpPost("reads")]
        // Ендпойнт читання інформації про запис в Solana
        public async Task<IActionResult> PostSolanaReads(ReadIncomingObj incomingObj) => (await _solanaCERT.SolanaReadsAsync(incomingObj.PubKey)).Result;

        [HttpPost("list")]
        // Ендпойнт отримання списку записів Solana
        public async Task<IActionResult> PostSolanaSeeds(SolanaSeedListIncomingObj incomingObj) => (await _solanaCERT.GetSolanaSeeds(incomingObj)).Result;

        [HttpPost("history")]
        // Ендпойнт отримання списку історичних записів про насіння в Solana
        public async Task<IActionResult> PostOneSolanaSeed(SolanaSeedIncomingObj incomingObj) => (await _solanaCERT.OneSolanaSeed(incomingObj)).Result;
        /* */
    }


    // Користувацькі вхідні об'єкти
    /* */
    // Вхідна інформація, для читання запису з Solana
    public class ReadIncomingObj
    {
        [Required]
        [MinLength(3)]
        // Публичний ключ облікового запису
        public string PubKey { get; set; } = string.Empty;
    }

    // Вхідна інформація, для отримання списку записів в Solana
    public class SolanaSeedListIncomingObj
    {
        [Required]
        // Пагінарор записів
        public Paginator Paginator { get; set; } = new Paginator();

        [Required]
        [AllowedValues(-1, 0, 1, 2)]
        // Поле фільтрації по Treatment (чи оброблене насіння)
        public int TreatmentType { get; set; } = -1;
    }

    // Вхідна інформація, для отримання історії записів по насінню та читання останнього з них з Solana
    public class SolanaSeedIncomingObj
    {
        [Required]
        [MinLength(3)]
        // Ідентифікатор запсиу в історії
        public string HistoryKey { get; set; } = string.Empty;
    }
    /* */
}
