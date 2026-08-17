using Bumptech.Glide.Load.Model;
using SQLite;
using TradeFlow.Models;

namespace TrainiumNeon.Data
{
    public class DatabaseService
    {
        // Propiedades privadas
        private readonly SQLiteAsyncConnection _db;
        private bool _initialized;

        // Constructor
        public DatabaseService(string dbPath)
        {
            _db = new SQLiteAsyncConnection(dbPath);
        }

        // Metodo publico para obtener la conexion a la DB
        public SQLiteAsyncConnection Connection()
        {
            return _db;
        }

        // Task asincrona para inicializar la DB
        public async Task InitializeAsync()
        {
            // Si la db ya esta inicializada salgo del metodo
            if (_initialized)
            {
                return;
            }

            // Creo las tablas de la DB
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