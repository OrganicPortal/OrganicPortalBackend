using Microsoft.AspNetCore.Mvc;
using OrganicPortalBackend.Services;

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

        //[HttpGet("confirmations")]
        //// Надсилає запит на формування довіреного запису про насіння
        //public async Task<IActionResult> SolanaConfirmation([FromQuery] long seedId, [FromQuery] long companyId) => (await _solanaCERT.SendSeedToSolanaAsync(seedId)).Result;

        [HttpPost("reads")]
        // Надсилає запит на формування довіреного запису про насіння
        public async Task<IActionResult> SolanaReads(ReadIncomingObj incomingObj) => (await _solanaCERT.SolanaReadsAsync(incomingObj.PubKey)).Result;
    }

    public class ReadIncomingObj
    {
        public string PubKey { get; set; } = string.Empty;
    }
}
