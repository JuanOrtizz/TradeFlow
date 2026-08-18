using TradeFlow.Models;

namespace TradeFlow.Data.Repositories
{
    public interface IFacturaRepositorio
    {
        Task<List<FacturaModel>> ObtenerTodasAsync();
        Task<FacturaModel?> ObtenerPorIdAsync(int id);
        Task<List<DetalleFacturaModel>> ObtenerDetallesAsync(int facturaId);
        Task<int> GuardarAsync(FacturaModel factura);
        Task<int> EliminarAsync(FacturaModel factura);
        Task<FacturaModel> RegistrarAsync(ClienteModel cliente, DetalleFacturaModel detalles);
        Task<List<FacturaModel>> ObtenerPorClienteAsync(int clienteId);
        Task<List<FacturaModel>> ObtenerPorFechaAsync(DateTime fecha);
    }
}
