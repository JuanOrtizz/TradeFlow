using TradeFlow.Models;

namespace TradeFlow.Data.Repositories
{
    public class FacturaRepository : IFacturaRepositorio
    {
        private readonly DatabaseService _db;

        public FacturaRepository(DatabaseService db)
        {
            _db = db;
        }

        public Task<List<FacturaModel>> ObtenerTodasAsync()
        {
            throw new NotImplementedException();
        }

        public Task<FacturaModel?> ObtenerPorIdAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<List<DetalleFacturaModel>> ObtenerDetallesAsync(int facturaId)
        {
            throw new NotImplementedException();
        }

        public Task<int> GuardarAsync(FacturaModel factura)
        {
            throw new NotImplementedException();
        }

        public Task<int> EliminarAsync(FacturaModel factura)
        {
            throw new NotImplementedException();
        }
    }
}
