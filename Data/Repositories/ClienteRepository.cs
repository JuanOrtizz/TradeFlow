using SQLite;
using TradeFlow.Models;

namespace TradeFlow.Data.Repositories
{
    public class ClienteRepository : IClienteRepository
    {
        private readonly SQLiteAsyncConnection _db;

        public ClienteRepository(DatabaseService db)
        {
            _db = db.Connection();
        }

        public async Task<IReadOnlyList<ClienteModel>> BuscarPorNombreAsync(string nombre)
        {
            var termino = nombre.Trim();

            var candidatos = await _db.QueryAsync<ClienteModel>(
                "SELECT * FROM ClienteModel WHERE Nombre LIKE ?",
                $"%{termino}%");

            return candidatos.Where(c => c.Nombre.IndexOf(termino, StringComparison.OrdinalIgnoreCase) >= 0).ToList();
        }

        public async Task<int> EliminarAsync(ClienteModel cliente)
        {
            var result = await _db.DeleteAsync(cliente);
            if (result > 0)
            {
                return result;
            }
            else
            {
                throw new Exception("Error al eliminar el cliente.");
            }
        }

        public async Task<bool> ExisteNombreAsync(string nombre, int idExcluido = 0)
        {
            var normalizado = nombre.Trim().ToLower();
            return await _db.Table<ClienteModel>()
                .Where(c => c.Nombre.ToLower() == normalizado && c.Id != idExcluido)
                .CountAsync() > 0;
        }

        public async Task<int> GuardarAsync(ClienteModel cliente)
        {
            var result = await _db.UpdateAsync(cliente);
            if (result > 0)
            {
                return result;
            }
            else
            {
                throw new Exception("Error al guardar el cliente.");
            }

        }

        public async Task<ClienteModel?> ObtenerPorIdAsync(int id)
        {
            return await _db.Table<ClienteModel>().Where(c => c.Id == id).FirstOrDefaultAsync();
        }

        public async Task<IReadOnlyList<ClienteModel>> ObtenerPorLocalidadAsync(int localidadId)
        {
            return await _db.Table<ClienteModel>().Where(c => c.LocalidadId == localidadId).ToListAsync();
        }

        public async Task<IReadOnlyList<ClienteModel>> ObtenerTodosAsync()
        {
            return await _db.Table<ClienteModel>().ToListAsync();
        }

        public async Task<ClienteModel> RegistrarAsync(string nombre, string telefono, string direccion, LocalidadModel localidad)
        {
            var cliente = new ClienteModel
            {
                Nombre = nombre,
                Telefono = telefono,
                Direccion = direccion,
                LocalidadId = localidad.Id
            };

            var result = await _db.InsertAsync(cliente);
            if (result > 0)
            {
                return cliente;
            }
            else
            {
                throw new Exception("Error al registrar el cliente.");
            }

        }
    }
}