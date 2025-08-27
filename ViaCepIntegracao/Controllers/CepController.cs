using Microsoft.AspNetCore.Mvc;
using System.Collections;
using ViaCepIntegracao.Exceptions;
using ViaCepIntegracao.Interfaces;
using ViaCepIntegracao.Models;
using ViaCepIntegracao.Models.DTO;

namespace ViaCepIntegracao.Controllers
{
    [Route("api/endereco")]
    [ApiController]
    public class CepController : Controller
    {

        private readonly IViaCepService _viaCep;
        private readonly ILogger<CepController> _log;
        private readonly IHistorico _historico;

        public CepController(IViaCepService viaCep, ILogger<CepController> log, IHistorico historico)
        {
            _viaCep = viaCep;
            _log = log;
            _historico = historico;
        }


        /// <summary>
        /// Busca pelo endereco, de acordo com os parametros que são passados,
        /// filtrando a busca e retornandos uma lista de endereços.
        /// </summary>
        /// <param name="uf">O uf filtra o endereço pela unidade federal.</param>
        /// <param name="cidade">cidade filtra o endereço pela cidade.</param>
        /// <param name="logradouro">logradouro filtra endereço por logradouro.</param>
        /// <returns>Ok() em caso de SUCESSO com a integração da API ViaCep</returns>
        /// <returns>BadRequest() em casa de FRACASSO com a integração da API ViaCep</returns>
        [HttpGet("{uf}/{cidade}/{logradouro}")]
        public async Task<ActionResult<ViaCepModel>> BuscarEndereco(string uf, string cidade, string logradouro)
        {
            if (uf == " " || uf == null || uf.Length != 2)
            {
                return BadRequest("O campo uf deve ter exatamente dois caracteres");
            }
            if (cidade == " " || cidade == null || cidade.Length < 2)
            {
                return StatusCode(400, "UF deve ter exatamente 2 caracteres ");
            }
            if (logradouro == " " || logradouro == null || logradouro.Length < 3)
            {
                return BadRequest("O campo logradouro deve ter exatamente dois caracteres");
            }
            try
            {
                var responseData = await _viaCep.ObterEnderecoPorUfCidadeLogradouroAsync(uf, cidade, logradouro);
                _historico.Registrar(new ViaCepDTO(uf, cidade, logradouro));

                if (!responseData.Any())
                {
                    return BadRequest("Cep não encontrado.");
                }
            return Ok(responseData);
            }
            catch (ApiViaCepException e)
            {
                _log.LogError(e, "Falha na conexão com a API");
                throw new ApiViaCepException("Erro ao processar dados da API", e);
            }
        }


        /// <summary>
        /// Busca os 10 ultimos que foram registrados.
        /// </summary>
        /// <returns>Ok para listas cheias ou vazias</returns>
        [HttpGet("historico")]
        public IActionResult BuscarHistorico()
        {
            var ultimas = _historico.ListarUltimas10();

            return Ok(ultimas);
        }
    }
}
