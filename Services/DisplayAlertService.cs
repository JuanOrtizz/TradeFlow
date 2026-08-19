using TradeFlow.Services;

namespace TradeFlow.Services
{
    public class DisplayAlertService : IDisplayAlertService
    {
        // Task asincrona para mostrar una alerta simple
        public async Task MostrarAlertAsync(string titulo, string mensaje, string cancelar)
        {
            var page = ObtenerPaginaActual();
            if (page != null)
            {
                await page.DisplayAlert(titulo, mensaje, cancelar);
            }
        }

        // Task asincrona para mostrar una alerta con confirmacion
        public async Task<bool> MostrarAlertConConfirmacionAsync(string titulo, string mensaje, string aceptar, string cancelar)
        {
            var page = ObtenerPaginaActual();
            if (page != null)
            {
                return await page.DisplayAlert(titulo, mensaje, aceptar, cancelar);
            }
            return false;
        }

        // Metodo privado para obtener la pagina actual
        private Page? ObtenerPaginaActual()
        {
            return Application.Current?.Windows.FirstOrDefault()?.Page;
        }
    }
}