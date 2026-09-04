using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using TradeFlow.Data.Repositories;
using TradeFlow.Services;

namespace TradeFlow.ViewModels
{
    public class AgregarLocalidadViewModel : INotifyPropertyChanged
    {
        private readonly ILocalidadRepository _localidadRepository;
        private readonly IDisplayAlertService _displayAlertService;
        private readonly IValidacionesService _validacionesService;

        // Propiedades privadas
        private string _nombre = string.Empty;
        private bool _hayErrorEnNombre;
        private string _errorNombre = string.Empty;
        private bool _isBusy;

        // Propiedades públicas
        public string Nombre
        {
            get => _nombre;
            set
            {
                var nuevoValor = value?.Trim() ?? string.Empty;
                if (_nombre != nuevoValor)
                {
                    _nombre = nuevoValor;
                    OnPropertyChanged(nameof(Nombre));
                }
            }
        }
        public bool HayErrorEnNombre
        {
            get => _hayErrorEnNombre;
            set
            {
                if (_hayErrorEnNombre != value)
                {
                    _hayErrorEnNombre = value;
                    OnPropertyChanged();
                }
            }
        }

        public string ErrorNombre
        {
            get => _errorNombre;
            set
            {
                var nuevoValor = value?.Trim() ?? string.Empty;
                if (_errorNombre != nuevoValor)
                {
                    _errorNombre = nuevoValor;
                    OnPropertyChanged(nameof(ErrorNombre));
                }
            }
        }

        public bool IsBusy
        {
            get => _isBusy;
            set
            {
                if (_isBusy != value)
                {
                    _isBusy = value;
                    OnPropertyChanged(nameof(IsBusy));
                }
            }
        }

        // Comandos
        public ICommand RegistroLocalidadCommand { get; }
        public ICommand VolverCommand { get; }
        public ICommand LimpiarErrorCommand { get; }

        public AgregarLocalidadViewModel(ILocalidadRepository localidadRepository, IDisplayAlertService displayAlertService, IValidacionesService validacionesService)
        {
            _localidadRepository = localidadRepository;
            _displayAlertService = displayAlertService;
            _validacionesService = validacionesService;
            RegistroLocalidadCommand = new Command(async () => await GuardarAsync());
            VolverCommand = new Command(async () => await Shell.Current.GoToAsync(".."));
            LimpiarErrorCommand = new Command<string>(LimpiarError);
        }

        private void LimpiarError(string campo)
        {
            if (campo == "Nombre")
            {
                HayErrorEnNombre = false;
                ErrorNombre = string.Empty;
            }
        }

        public async Task GuardarAsync()
        {
            // Validar el nombre de la localidad
            ErrorNombre = _validacionesService.ValidarCampoVacio(Nombre);
            HayErrorEnNombre = !string.IsNullOrEmpty(ErrorNombre);
            if (HayErrorEnNombre)
            {
                return;
            }

            try
            {
                IsBusy = true;

                // Validar duplicados
                if (await _localidadRepository.ExisteNombreAsync(Nombre))
                {
                    ErrorNombre = "Ya existe una localidad registrada con este nombre";
                    HayErrorEnNombre = true;
                    return;
                }

                await _localidadRepository.RegistrarAsync(Nombre.Trim());

                await _displayAlertService.MostrarAlertAsync("Éxito", "Localidad registrada correctamente", "OK");

                await Shell.Current.GoToAsync("..");
            }
            catch (Exception)
            {
                await _displayAlertService.MostrarAlertAsync("Error", "No se pudo registrar la localidad", "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
