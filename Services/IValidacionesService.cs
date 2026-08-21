
namespace TradeFlow.Services
{
    public interface IValidacionesService
    {
        // Metodo para validar que un campo no este vacio
        string ValidarCampoVacio(string campo);

        // Metodo para validar que un select no este vacio
        string ValidarSeleccion<T>(T? seleccion, string nombreCampo);
    }
}
