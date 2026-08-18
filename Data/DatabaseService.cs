using SQLite;
using TradeFlow.Models;

namespace TradeFlow.Data
{
    public class DatabaseService
    {
        private readonly SQLiteAsyncConnection _db;
        private bool _initialized;

        public DatabaseService(string dbPath)
        {
            _db = new SQLiteAsyncConnection(dbPath);
        }

        public SQLiteAsyncConnection Connection()
        {
            return _db;
        }

        public async Task InitializeAsync()
        {

            if (_initialized)
            {
                return;
            }

            await _db.CreateTableAsync<LocalidadModel>();
            await _db.CreateTableAsync<ClienteModel>();
            await _db.CreateTableAsync<ProductoModel>();
            await _db.CreateTableAsync<FacturaModel>();
            await _db.CreateTableAsync<DetalleFacturaModel>();

            // Inicializo la variable para no volver a inicializar
            _initialized = true;
        }
    }
}
