using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.Extensions.Caching.Memory;
using Refit;
using System.Net;
using ViaCepIntegracao.Exceptions;
using ViaCepIntegracao.Interfaces;
using ViaCepIntegracao.Models;
using ViaCepIntegracao.Refit;

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

        private string ChaveString(string uf, string cidade, string logradouro)
        {
            return $"{uf.ToUpperInvariant()}{cidade.ToLowerInvariant()}{logradouro.ToLowerInvariant()}";
        }

        public async Task<List<ViaCepModel>> ObterEnderecoPorUfCidadeLogradouroAsync(string uf, string cidade, string logradouro)
        {
            HttpResponseMessage response;
            var status = HttpStatusCode.NoContent;

            var path = $"/ws/{Uri.EscapeDataString(uf)}/{Uri.EscapeDataString(cidade)}/{Uri.EscapeDataString(logradouro)}/json";
            var chave = ChaveString(uf, cidade, logradouro);

            if (_cache.TryGetValue<List<ViaCepModel>>(chave, out var valor))
            {
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
