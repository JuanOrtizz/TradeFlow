using SQLite;
using TradeFlow.Models;

namespace TradeFlow.Data.Repositories
{
    public class ProductoRepository : IProductoRepository
    {
        private readonly SQLiteAsyncConnection _db;

        public ProductoRepository(DatabaseService db)
        {
            _db = db.Connection();
        }

        public async Task<IReadOnlyList<ProductoModel>> BuscarPorNombreAsync(string nombre)
        {
            return await _db.Table<ProductoModel>().Where(p => p.Nombre.Contains(nombre)).ToListAsync();
        }

        public async Task<int> EliminarAsync(ProductoModel producto)
        {
            var result = await _db.DeleteAsync(producto);
            if (result > 0)
            {
                return result;
            }
            else
            {
                throw new Exception("Error al eliminar el producto.");
            }
        }

        public async Task<int> GuardarAsync(ProductoModel producto)
        {
            var result = await _db.UpdateAsync(producto);
            if (result > 0)
            {
                return result;
            }
            else
            {
                throw new Exception("Error al guardar el producto.");
            }
        }

        public async Task<ProductoModel?> ObtenerPorIdAsync(int id)
        {
            return await _db.Table<ProductoModel>().Where(p => p.Id == id).FirstOrDefaultAsync();
        }

        public async Task<IReadOnlyList<ProductoModel>> ObtenerTodosAsync()
        {
            return await _db.Table<ProductoModel>().ToListAsync();
        }

        public async Task<ProductoModel> RegistrarAsync(string nombre, string codigo, decimal precio)
        {
            var producto = new ProductoModel
            {
                Nombre = nombre,
                Codigo = codigo,
                Precio = precio
            };

            var result = await _db.InsertAsync(producto);
            if (result > 0)
            {
                return producto;
            }
            else
            {
                throw new Exception("Error al registrar el producto.");
            }
        }
    }
}
