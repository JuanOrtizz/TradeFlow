
namespace TradeFlow.Services
{
    public interface IDisplayAlertService
    {
        Task MostrarAlertAsync(string titulo, string mensaje, string cancelar);
        Task<bool> MostrarAlertConConfirmacionAsync(string titulo, string mensaje, string aceptar, string cancelar);
    }
}
