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
            return await _db.Table<ClienteModel>().Where(c => c.Nombre.Contains(nombre)).ToListAsync();
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