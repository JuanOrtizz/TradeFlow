using SQLite;
using TradeFlow.Models;

namespace TradeFlow.Data.Repositories
{
    public class LocalidadRepository : ILocalidadRepository
    {
        private readonly SQLiteAsyncConnection _db;

        public LocalidadRepository(DatabaseService db)
        {
            _db = db.Connection();
        }

        public async Task<int> EliminarAsync(LocalidadModel localidad)
        {
            var result = await _db.DeleteAsync(localidad);
            if (result > 0)
            {
                return result;
            }
            else
            {
                throw new Exception("Error al eliminar la localidad.");
            }
        }

        public async Task<bool> ExisteNombreAsync(string nombre, int idExcluido = 0)
        {
            var normalizado = nombre.Trim().ToLower();
            return await _db.Table<LocalidadModel>()
                .Where(l => l.Nombre.ToLower() == normalizado && l.Id != idExcluido)
                .CountAsync() > 0;
        }

        public async Task<int> GuardarAsync(LocalidadModel localidad)
        {
            var result = await _db.UpdateAsync(localidad);
            if (result > 0)
            {
                return result;
            }
            else
            {
                throw new Exception("Error al guardar la localidad.");
            }
        }

        public async Task<LocalidadModel> ObtenerPorIdAsync(int id)
        {
            return await _db.Table<LocalidadModel>().Where(l => l.Id == id).FirstOrDefaultAsync();
        }

        public async Task<IReadOnlyList<LocalidadModel>> ObtenerTodasAsync()
        {
            return await _db.Table<LocalidadModel>().ToListAsync();
        }

        public async Task<LocalidadModel> RegistrarAsync(string nombre)
        {
            var localidad = new LocalidadModel
            {
                Nombre = nombre
            };

            var result = await _db.InsertAsync(localidad);
            if (result > 0)
            {
                return localidad;
            }
            else
            {
                throw new Exception("Error al registrar la localidad.");
            }
        }
    }
}
