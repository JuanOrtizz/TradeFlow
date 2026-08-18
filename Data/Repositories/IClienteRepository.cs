using TradeFlow.Models;

namespace TradeFlow.Data.Repositories
{
    public interface IClienteRepository
    {
        Task<IReadOnlyList<ClienteModel>> ObtenerTodosAsync();
        Task<ClienteModel?> ObtenerPorIdAsync(int id);
        Task<int> GuardarAsync(ClienteModel cliente);
        Task<int> EliminarAsync(ClienteModel cliente);
        Task<IReadOnlyList<ClienteModel>> ObtenerPorLocalidadAsync(int localidadId);
        Task<IReadOnlyList<ClienteModel>> BuscarPorNombreAsync(string nombre);
        Task<ClienteModel> RegistrarAsync(string nombre, string telefono, string direccion, LocalidadModel localidad);

    }
}
