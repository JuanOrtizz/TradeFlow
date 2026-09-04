using SQLite;
using TradeFlow.Models;

namespace TradeFlow.Data
{
    public class DatabaseService
    {
        private readonly string _dbPath;
        private SQLiteAsyncConnection _db;
        private bool _initialized;

        public string DbPath => _dbPath;

        public DatabaseService(string dbPath)
        {
            _dbPath = dbPath;
            _db = new SQLiteAsyncConnection(dbPath);
        }

        public SQLiteAsyncConnection Connection()
        {
            return _db;
        }

        public async Task CerrarConexionAsync()
        {
            await _db.CloseAsync();
            _initialized = false;
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
