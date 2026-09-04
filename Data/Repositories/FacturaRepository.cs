using SQLite;
using TradeFlow.Models;

namespace TradeFlow.Data.Repositories
{
    public class FacturaRepository : IFacturaRepository
    {
        private readonly SQLiteAsyncConnection _db;

        public FacturaRepository(DatabaseService db)
        {
            _db = db.Connection();
        }

        public async Task<int> EliminarAsync(FacturaModel factura)
        {
            await _db.ExecuteAsync("DELETE FROM DetalleFacturaModel WHERE FacturaId = ?", factura.Id);

            var result = await _db.DeleteAsync(factura);
            if (result > 0)
            {
                return result;
            }
            else
            {
                throw new Exception("Error al eliminar la factura.");
            }
        }

        public async Task<int> GuardarAsync(FacturaModel factura)
        {
            var result = await _db.UpdateAsync(factura);
            if (result > 0)
            {
                return result;
            }
            else
            {
                throw new Exception("Error al guardar la factura.");
            }
        }

        public async Task<IReadOnlyList<DetalleFacturaModel>> ObtenerDetallesAsync(int facturaId)
        {
            return await _db.Table<DetalleFacturaModel>().Where(d => d.FacturaId == facturaId).ToListAsync();
        }

        public async Task<IReadOnlyList<FacturaModel>> ObtenerPorClienteAsync(int clienteId)
        {
            return await _db.Table<FacturaModel>().Where(f => f.ClienteId == clienteId).ToListAsync();
        }

        public async Task<int> ContarPorClienteAsync(int clienteId)
        {
            return await _db.Table<FacturaModel>().Where(f => f.ClienteId == clienteId).CountAsync();
        }

        public async Task<IReadOnlyList<FacturaModel>> BuscarPorNumeroAsync(string numero)
        {
            var termino = numero.Trim();
            return await _db.QueryAsync<FacturaModel>(
                "SELECT * FROM FacturaModel WHERE CAST(Id AS TEXT) LIKE ?",
                $"%{termino}%");
        }

        public async Task<IReadOnlyList<FacturaModel>> ObtenerPorFechaAsync(DateTime fecha)
        {
            // sqlite guarda las fechas como ticks enteros, asi que DATE() de SQL no las entiende.
            // Comparo por rango: >= inicio del dia y < inicio del dia siguiente
            var desde = fecha.Date;
            var hasta = desde.AddDays(1);

            return await _db.Table<FacturaModel>()
                .Where(f => f.Fecha >= desde && f.Fecha < hasta)
                .ToListAsync();
        }

        public async Task<FacturaModel?> ObtenerPorIdAsync(int id)
        {
            return await _db.Table<FacturaModel>().Where(f => f.Id == id).FirstOrDefaultAsync();
        }

        public async Task<IReadOnlyList<FacturaModel>> ObtenerTodasAsync()
        {
            return await _db.Table<FacturaModel>().ToListAsync();
        }

        public async Task<FacturaModel> RegistrarAsync(ClienteModel cliente, List<DetalleFacturaModel> items)
        {
            var factura = new FacturaModel
            {
                ClienteId = cliente.Id,
                Fecha = DateTime.Now,
                Total = items.Sum(i => i.Subtotal)
            };

            var result = await _db.InsertAsync(factura);
            if (result > 0)
            {
                foreach (var item in items)
                {
                    item.FacturaId = factura.Id;
                    await _db.InsertAsync(item);
                }
                return factura;
            }
            else
            {
                throw new Exception("Error al registrar la factura.");
            }
        }

        public async Task<IReadOnlyList<FacturaModel>> ObtenerUltimasDiezAsync()
        {
            return await _db.Table<FacturaModel>().OrderByDescending(f => f.Fecha).Take(10).ToListAsync();
        }
    }
}
