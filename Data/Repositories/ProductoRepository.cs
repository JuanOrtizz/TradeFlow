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

        public async Task<IReadOnlyList<ProductoModel>> BuscarAsync(string termino)
        {
            var texto = termino.Trim();

            var candidatos = await _db.QueryAsync<ProductoModel>(
                "SELECT * FROM ProductoModel WHERE Nombre LIKE ? OR Codigo LIKE ?",
                $"%{texto}%", $"%{texto}%");

            return candidatos
                .Where(p => p.Nombre.IndexOf(texto, StringComparison.OrdinalIgnoreCase) >= 0
                         || (p.Codigo ?? string.Empty).IndexOf(texto, StringComparison.OrdinalIgnoreCase) >= 0)
                .OrderByDescending(p => p.Activo)
                .ThenBy(p => p.Nombre)
                .ToList();
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

        public async Task<bool> ExisteNombreAsync(string nombre, int idExcluido = 0)
        {
            var normalizado = nombre.Trim().ToLower();
            return await _db.Table<ProductoModel>()
                .Where(p => p.Nombre.ToLower() == normalizado && p.Id != idExcluido)
                .CountAsync() > 0;
        }

        public async Task<bool> ExisteCodigoAsync(string codigo, int idExcluido = 0)
        {
            var normalizado = codigo.Trim().ToLower();
            return await _db.Table<ProductoModel>()
                .Where(p => p.Codigo.ToLower() == normalizado && p.Id != idExcluido)
                .CountAsync() > 0;
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
            var productos = await _db.Table<ProductoModel>().ToListAsync();
            return productos
                .OrderByDescending(p => p.Activo)
                .ThenBy(p => p.Nombre)
                .ToList();
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
