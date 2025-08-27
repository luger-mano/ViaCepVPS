using ViaCepIntegracao.Models;

namespace ViaCepIntegracao.Interfaces
{
    public interface IViaCepService
    {
        Task<List<ViaCepModel>> ObterEnderecoPorUfCidadeLogradouroAsync(string uf, string cidade, string logradouro);
    }
}
