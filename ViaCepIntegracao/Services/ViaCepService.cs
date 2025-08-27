using Microsoft.Extensions.Caching.Memory;
using System.Net;
using ViaCepIntegracao.Exceptions;
using ViaCepIntegracao.Interfaces;
using ViaCepIntegracao.Models;

namespace ViaCepIntegracao.Services
{
    public class ViaCepService : IViaCepService
    {

        private readonly HttpClient _client;
        private readonly ILogger<ViaCepService> _log;
        private readonly IMemoryCache _cache;

        public ViaCepService(HttpClient client, ILogger<ViaCepService> log, IMemoryCache cache)
        {
            _client = client;
            _log = log;
            _cache = cache;
        }

        /// <summary>
        /// Retorna uma "chave" para que seja utilizada no cache como identificação e
        /// formata a string para caixa alta e baixa.
        /// </summary>
        /// <param name="uf"></param>
        /// <param name="cidade"></param>
        /// <param name="logradouro"></param>
        /// <returns>Retorna a string com os parametros formatados.</returns>
        private string ChaveString(string uf, string cidade, string logradouro)
        {
            return $"{uf.ToUpperInvariant()}{cidade.ToLowerInvariant()}{logradouro.ToLowerInvariant()}";
        }

        /// <summary>
        /// Primeiro verifica se há um cache em memória buscando pela "chave" e retorna o valor caso true, se não
        /// Faz a conectividade com a API através do caminho URI, se obter sucesso continua, se não, retorna uma lista vazia.
        /// Insere em uma lista a reposta de um Json desserializado, se não, retorna uma lista vazia.
        /// Setta no cache os valores da "chave", a lista Json e o tempo que o cache ficará disponível (10 minutos).
        /// </summary>
        /// <param name="uf">Busca endereço através do uf como filtro.</param>
        /// <param name="cidade">Busca endereço através da cidade como filtro.</param>
        /// <param name="logradouro">Busca endereço através do logradouro como filtro.</param>
        /// <returns>Retorna a lista do Json desserializado com o resultado da consulta através dos parametros.</returns>
        public async Task<List<ViaCepModel>> ObterEnderecoPorUfCidadeLogradouroAsync(string uf, string cidade, string logradouro)
        {
            HttpResponseMessage response;
            var status = HttpStatusCode.NoContent;

            var path = $"/ws/{Uri.EscapeDataString(uf)}/{Uri.EscapeDataString(cidade)}/{Uri.EscapeDataString(logradouro)}/json";
            var chave = ChaveString(uf, cidade, logradouro);

            if (_cache.TryGetValue<List<ViaCepModel>>(chave, out var valor))
            {
                _log.LogInformation("Pegou do cache");
                return valor;
            }
            try
            {
                response = await _client.GetAsync(path);
                status = response.StatusCode;
            }
            catch (ApiViaCepException e)
            {
                _log.LogError(e, "Caminho errado, api retornou {status}", status);
                throw new ApiViaCepException("Erro no caminho da API ViaCep", e);
            }

            if (status == HttpStatusCode.NoContent || status == HttpStatusCode.NotFound)
            {
                _log.LogInformation("Conteudo não foi encontrado.{status}", status);
                return [];
            }
            if (!response.IsSuccessStatusCode)
            {
                _log.LogWarning("Resposta da API ViaCep sem sucesso. {status}", status);
                return [];
            }

            List<ViaCepModel>? result = null;
            try
            {
                var jsonToAsync = await response.Content.ReadFromJsonAsync<List<ViaCepModel>>();
                result = jsonToAsync ?? [];
            }
            catch (Exception e)
            {
                _log.LogError("Conteudo não foi desserializado corretamente.{status}", status);
                throw new Exception(e.Message);
            }

            _cache.Set(chave, result, new MemoryCacheEntryOptions { AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10)});

            return result;
        }
    }
}
