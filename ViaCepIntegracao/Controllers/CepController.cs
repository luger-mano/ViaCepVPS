using Microsoft.AspNetCore.Mvc;
using ViaCepIntegracao.Interfaces;
using ViaCepIntegracao.Models;
using ViaCepIntegracao.Refit;
using ViaCepIntegracao.Services;

namespace ViaCepIntegracao.Controllers
{
    [Route("api/endereco")]
    [ApiController]
    public class CepController : Controller
    {

        private readonly IViaCepService _viaCep;
        private readonly ILogger<CepController> _log;

        public CepController(IViaCepService viaCep, ILogger<CepController> log)
        {
            _viaCep = viaCep;
            _log = log;
        }

        [HttpGet("{uf}/{cidade}/{logradouro}")]
        public async Task<ActionResult<ViaCepModel>> BuscarEndereco(string uf, string cidade, string logradouro)
        {
            var response = await _viaCep.ObterEnderecoPorUfCidadeLogradouroAsync(uf, cidade, logradouro);

            if (response == null)
            {
                return BadRequest("Cep não encontrado.");
            }

            return Ok(response);
        }
    }
}
