using TradeFlow.Models;

namespace TradeFlow.Services
{
    public interface IImpresionService
    {
        string GenerarHtmlFactura(FacturaModel factura, IReadOnlyList<DetalleFacturaModel> items);
        string GenerarHtmlCatalogo(IReadOnlyList<ProductoModel> productos);
        Task ImprimirAsync(object controlWebView);
        Task<string> ExportarAPdfAsync(object controlWebView, string html, string nombreArchivo);
        Task<string> GenerarCatalogoPdfAsync(IReadOnlyList<ProductoModel> productos);
    }
}
