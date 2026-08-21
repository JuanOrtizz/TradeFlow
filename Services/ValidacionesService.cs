
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

        // Metodo para validar que un select no este vacio
        public string ValidarSeleccion<T>(T? seleccion, string nombreCampo)
        {
            return seleccion == null ? $"Seleccione una {nombreCampo}" : string.Empty;
        }
    }
}
