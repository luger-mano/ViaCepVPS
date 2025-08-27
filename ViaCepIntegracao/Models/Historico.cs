using System.Collections.Concurrent;
using System.Collections.Immutable;
using ViaCepIntegracao.Interfaces;
using ViaCepIntegracao.Models.DTO;

namespace ViaCepIntegracao.Models
{
    public class Historico : IHistorico
    {

        private ImmutableList<ViaCepDTO> _historico = ImmutableList<ViaCepDTO>.Empty;

        /// <summary>
        /// Registra na lista _historico os dados necessários para o objeto ViaCepDTO.
        /// </summary>
        /// <param name="dto">O objeto do record com os dados fornecidos</param>
        public void Registrar(ViaCepDTO dto)
        {
            if (_historico.Count >= 1000)
            {
                _historico.RemoveAt(0);
            }
            _historico = _historico.Add(dto);
        }

        /// <summary>
        /// Retorna as ultimas 10 consultas que foram feitas, IEnumerable
        /// é responsável pela leitura dos dados sendo mais performático 
        /// nesse contexto.
        /// </summary>
        /// <returns>Ultimos 10 da lista de _historico</returns>
        public IEnumerable<ViaCepDTO> ListarUltimas10()
        {
            return _historico.TakeLast(10);
        }

    }
}
