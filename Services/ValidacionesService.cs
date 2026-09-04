using System.Globalization;

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

        // Metodo para validar que el precio sea mayor a cero
        public string ValidarPrecio(decimal precio)
        {
            return precio <= 0 ? "El precio debe ser mayor a 0." : string.Empty;
        }

        // Metodo para convertir un texto a decimal aceptando coma o punto como separador
        public bool TryObtenerDecimal(string texto, out decimal valor)
        {
            valor = 0;
            if (string.IsNullOrWhiteSpace(texto))
            {
                return false;
            }
            var normalizado = texto.Trim().Replace(",", ".");
            return decimal.TryParse(normalizado, NumberStyles.Number, CultureInfo.InvariantCulture, out valor);
        }
    }
}
