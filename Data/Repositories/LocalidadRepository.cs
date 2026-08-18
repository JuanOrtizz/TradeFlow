using TradeFlow.Models;

namespace TradeFlow.Data.Repositories
{
    public class LocalidadRepository : ILocalidadRepository
    {
        private readonly DatabaseService _db;

        public LocalidadRepository(DatabaseService db)
        {
            _db = db;
        }

        public Task<List<LocalidadModel>> ObtenerTodasAsync()
        {
            throw new NotImplementedException();
        }

        public Task<LocalidadModel?> ObtenerPorIdAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<int> GuardarAsync(LocalidadModel localidad)
        {
            throw new NotImplementedException();
        }

        public Task<int> EliminarAsync(LocalidadModel localidad)
        {
            throw new NotImplementedException();
        }
    }
}
