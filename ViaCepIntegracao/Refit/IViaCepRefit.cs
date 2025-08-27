using Refit;
using ViaCepIntegracao.Models;

namespace ViaCepIntegracao.Refit
{
    public interface IViaCepRefit
    {
        [Get("/ws/{uf}/{cidade}/{logradouro}/json")]
        Task<ApiResponse<ViaCepModel>> ObterDadosViaCep([AliasAs("uf")]string uf, [AliasAs("cidade")] string cidade, [AliasAs("logradouro")] string logradouro);
    }
}
