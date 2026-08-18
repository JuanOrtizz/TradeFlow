using TradeFlow.Models;

namespace TradeFlow.Data.Repositories
{
    public class ProductoRepository : IProductoRepositorio
    {
        private readonly DatabaseService _db;

        public ProductoRepository(DatabaseService db)
        {
            _db = db;
        }

        public Task<List<ProductoModel>> ObtenerTodosAsync()
        {
            throw new NotImplementedException();
        }

        public Task<ProductoModel?> ObtenerPorIdAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<int> GuardarAsync(ProductoModel producto)
        {
            throw new NotImplementedException();
        }

        public Task<int> EliminarAsync(ProductoModel producto)
        {
            throw new NotImplementedException();
        }
    }
}
