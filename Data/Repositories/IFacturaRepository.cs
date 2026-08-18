using TradeFlow.Models;

namespace TradeFlow.Data.Repositories
{
    public interface IFacturaRepository
    {
        Task<IReadOnlyList<FacturaModel>> ObtenerTodasAsync();
        Task<FacturaModel?> ObtenerPorIdAsync(int id);
        Task<IReadOnlyList<DetalleFacturaModel>> ObtenerDetallesAsync(int facturaId);
        Task<int> GuardarAsync(FacturaModel factura);
        Task<int> EliminarAsync(FacturaModel factura);
        Task<FacturaModel> RegistrarAsync(ClienteModel cliente, List<DetalleFacturaModel> items);
        Task<IReadOnlyList<FacturaModel>> ObtenerPorClienteAsync(int clienteId);
        Task<IReadOnlyList<FacturaModel>> ObtenerPorFechaAsync(DateTime fecha);
    }
}
