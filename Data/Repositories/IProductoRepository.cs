using TradeFlow.Models;

namespace TradeFlow.Data.Repositories
{
    public interface IProductoRepository
    {
        Task<List<ProductoModel>> ObtenerTodosAsync();
        Task<ProductoModel?> ObtenerPorIdAsync(int id);
        Task<int> GuardarAsync(ProductoModel producto);
        Task<int> EliminarAsync(ProductoModel producto);
        Task<List<ProductoModel>> BuscarPorNombreAsync(string nombre);
        Task<ProductoModel> RegistrarAsync(string nombre, string codigo, decimal precio);
    }
}
