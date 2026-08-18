using TradeFlow.Models;

namespace TradeFlow.Data.Repositories
{
    public interface IClienteRepository
    {
        Task<List<ClienteModel>> ObtenerTodosAsync();
        Task<ClienteModel?> ObtenerPorIdAsync(int id);
        Task<int> GuardarAsync(ClienteModel cliente);
        Task<int> EliminarAsync(ClienteModel cliente);
        Task<List<ClienteModel>> ObtenerPorLocalidadAsync(int localidadId);
        Task<List<ClienteModel>> BuscarPorNombreAsync(string nombre);
        Task<ClienteModel> RegistrarAsync(string nombre, string telefono, string direccion, LocalidadModel localidad);

    }
}
