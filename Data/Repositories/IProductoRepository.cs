using TradeFlow.Models;

namespace TradeFlow.Data.Repositories
{
    public interface IProductoRepository
    {
        Task<IReadOnlyList<ProductoModel>> ObtenerTodosAsync();
        Task<ProductoModel?> ObtenerPorIdAsync(int id);
        Task<int> GuardarAsync(ProductoModel producto);
        Task<int> EliminarAsync(ProductoModel producto);
        Task<bool> ExisteNombreAsync(string nombre, int idExcluido = 0);
        Task<bool> ExisteCodigoAsync(string codigo, int idExcluido = 0);
        Task<IReadOnlyList<ProductoModel>> BuscarAsync(string termino);
        Task<ProductoModel> RegistrarAsync(string nombre, string codigo, decimal precio);
    }
}
