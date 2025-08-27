using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ViaCepIntegracao.Controllers
{

    [Route("api/health")]
    [ApiController]
    public class HealthCheckController : Controller
    {

        /// <summary>
        /// Busca para saber a saúde da requisição
        /// </summary>
        /// <returns>"Health" para requisção saudavel</returns>
        [HttpGet]
        public IActionResult BuscarHealth()
        {
            return Ok(new { status = "Health", timestamp = DateTime.UtcNow});
        }
    }
}
