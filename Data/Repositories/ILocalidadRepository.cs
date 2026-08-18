using TradeFlow.Models;

namespace TradeFlow.Data.Repositories
{
    public interface ILocalidadRepository
    {
        Task<IReadOnlyList<LocalidadModel>> ObtenerTodasAsync();
        Task<LocalidadModel> ObtenerPorIdAsync(int id);
        Task<LocalidadModel> RegistrarAsync(string nombre);
        Task<int> GuardarAsync(LocalidadModel localidad);
        Task<int> EliminarAsync(LocalidadModel localidad);
    }
}
