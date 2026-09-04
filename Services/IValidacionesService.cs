
namespace TradeFlow.Services
{
    public interface IValidacionesService
    {
        // Metodo para validar que un campo no este vacio
        string ValidarCampoVacio(string campo);

        // Metodo para validar que un select no este vacio
        string ValidarSeleccion<T>(T? seleccion, string nombreCampo);

        // Metodo para validar que el precio sea mayor a cero
        string ValidarPrecio(decimal precio);

        // Metodo para convertir un texto a decimal aceptando coma o punto como separador
        bool TryObtenerDecimal(string texto, out decimal valor);
    }
}
