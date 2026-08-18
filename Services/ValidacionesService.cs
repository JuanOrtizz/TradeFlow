
namespace TradeFlow.Services
{
    public class ValidacionesService : IValidacionesService
    {
        // Metodo para validar que un campo no este vacio
        public string ValidarCampoVacio(string campo)
        {
            if (string.IsNullOrWhiteSpace(campo))
            {
                return "El campo está vacío.";
            }
            return string.Empty;
        }
    }
}
