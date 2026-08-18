using TradeFlow.Models;

namespace TradeFlow.Data.Repositories
{
    public class ClienteRepository : IClienteRepository
    {
        private readonly DatabaseService _db;

        public ClienteRepository(DatabaseService db)
        {
            _db = db;
        }

        public Task<List<ClienteModel>> ObtenerTodosAsync()
        {
            throw new NotImplementedException();
        }

        public Task<ClienteModel?> ObtenerPorIdAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<int> GuardarAsync(ClienteModel cliente)
        {
            throw new NotImplementedException();
        }

        public Task<int> EliminarAsync(ClienteModel cliente)
        {
            throw new NotImplementedException();
        }
    }
}
