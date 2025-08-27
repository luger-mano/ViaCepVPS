using ViaCepIntegracao.Models.DTO;

namespace ViaCepIntegracao.Interfaces
{
    public interface IHistorico
    {
        void Registrar(ViaCepDTO dto);
        IEnumerable<ViaCepDTO> ListarUltimas10();
    }
}
