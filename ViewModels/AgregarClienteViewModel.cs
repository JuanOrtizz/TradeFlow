using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using TradeFlow.Data.Repositories;
using TradeFlow.Models;
using TradeFlow.Services;

namespace TradeFlow.ViewModels
{
    public class AgregarClienteViewModel
    {
        private readonly IClienteRepository _clienteRepository;
        private readonly ILocalidadRepository _localidadRepository;
        private readonly IDisplayAlertService _displayAlertService;
        private readonly IValidacionesService _validacionesService;

        private string _nombre = string.Empty;
        private string _telefono = string.Empty;
        private string _direccion = string.Empty;
        private LocalidadModel? _localidadSeleccionada;
        private bool _hayErrorEnNombre;
        private string _errorNombre = string.Empty;
        private bool _hayErrorEnTelefono;
        private string _errorTelefono = string.Empty;
        private bool _hayErrorEnDireccion;
        private string _errorDireccion = string.Empty;
        private bool _hayErrorEnLocalidad;
        private string _errorLocalidad = string.Empty;
        private bool _isBusy;

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

        public string Telefono
        {
            get => _telefono;
            set
            {
                var nuevoValor = value?.Trim() ?? string.Empty;
                if (_telefono != nuevoValor)
                {
                    _telefono = nuevoValor;
                    OnPropertyChanged(nameof(Telefono));
                }
            }
        }

        public string Direccion
        {
            get => _direccion;
            set
            {
                var nuevoValor = value?.Trim() ?? string.Empty;
                if (_direccion != nuevoValor)
                {
                    _direccion = nuevoValor;
                    OnPropertyChanged(nameof(Direccion));
                }
            }
        }

        public LocalidadModel? LocalidadSeleccionada
        {
            get => _localidadSeleccionada;
            set
            {
                if (_localidadSeleccionada != value)
                {
                    _localidadSeleccionada = value;
                    OnPropertyChanged(nameof(LocalidadSeleccionada));
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

        public bool HayErrorEnTelefono
        {
            get => _hayErrorEnTelefono;
            set
            {
                if (_hayErrorEnTelefono != value)
                {
                    _hayErrorEnTelefono = value;
                    OnPropertyChanged();
                }
            }
        }

        public string ErrorTelefono
        {
            get => _errorTelefono;
            set
            {
                var nuevoValor = value?.Trim() ?? string.Empty;
                if (_errorTelefono != nuevoValor)
                {
                    _errorTelefono = nuevoValor;
                    OnPropertyChanged(nameof(ErrorTelefono));
                }
            }
        }

        public bool HayErrorEnDireccion
        {
            get => _hayErrorEnDireccion;
            set
            {
                if (_hayErrorEnDireccion != value)
                {
                    _hayErrorEnDireccion = value;
                    OnPropertyChanged();
                }
            }
        }

        public string ErrorDireccion
        {
            get => _errorDireccion;
            set
            {
                var nuevoValor = value?.Trim() ?? string.Empty;
                if (_errorDireccion != nuevoValor)
                {
                    _errorDireccion = nuevoValor;
                    OnPropertyChanged(nameof(ErrorDireccion));
                }
            }
        }

        public bool HayErrorEnLocalidad
        {
            get => _hayErrorEnLocalidad;
            set
            {
                if (_hayErrorEnLocalidad != value)
                {
                    _hayErrorEnLocalidad = value;
                    OnPropertyChanged();
                }
            }
        }

        public string ErrorLocalidad
        {
            get => _errorLocalidad;
            set
            {
                var nuevoValor = value?.Trim() ?? string.Empty;
                if (_errorLocalidad != nuevoValor)
                {
                    _errorLocalidad = nuevoValor;
                    OnPropertyChanged(nameof(ErrorLocalidad));
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

        public ObservableCollection<LocalidadModel> ListaLocalidades { get; } = new ObservableCollection<LocalidadModel>();

        public ICommand RegistroClienteCommand { get; }

        public AgregarClienteViewModel(IClienteRepository clienteRepository, ILocalidadRepository localidadRepository, IDisplayAlertService displayAlertService, IValidacionesService validacionesService)
        {
            _clienteRepository = clienteRepository;
            _localidadRepository = localidadRepository;
            _displayAlertService = displayAlertService;
            _validacionesService = validacionesService;
            RegistroClienteCommand = new Command(async () => await GuardarAsync());
        }

        public async Task CargarLocalidadesAsync()
        {
            try
            {
                ListaLocalidades.Clear();
                var localidades = await _localidadRepository.ObtenerTodasAsync();
                foreach (var localidad in localidades)
                {
                    ListaLocalidades.Add(localidad);
                }
            }
            catch (Exception)
            {
                await _displayAlertService.MostrarAlertAsync("Error", "No se pudieron cargar las localidades", "OK");
            }
        }

        public async Task GuardarAsync()
        {
            try
            {
                IsBusy = true;

                ErrorNombre = _validacionesService.ValidarCampoVacio(Nombre);
                HayErrorEnNombre = !string.IsNullOrEmpty(ErrorNombre);

                ErrorTelefono = _validacionesService.ValidarCampoVacio(Telefono);
                HayErrorEnTelefono = !string.IsNullOrEmpty(ErrorTelefono);

                ErrorDireccion = _validacionesService.ValidarCampoVacio(Direccion);
                HayErrorEnDireccion = !string.IsNullOrEmpty(ErrorDireccion);

                ErrorLocalidad = _validacionesService.ValidarSeleccion(LocalidadSeleccionada, "localidad");
                HayErrorEnLocalidad = !string.IsNullOrEmpty(ErrorLocalidad);

                if (HayErrorEnNombre || HayErrorEnTelefono || HayErrorEnDireccion || HayErrorEnLocalidad)
                {
                    return;
                }

                await _clienteRepository.RegistrarAsync(Nombre, Telefono, Direccion, LocalidadSeleccionada);

                await _displayAlertService.MostrarAlertAsync("Éxito", "Cliente registrado correctamente", "OK");

                Nombre = string.Empty;
                Telefono = string.Empty;
                Direccion = string.Empty;
                LocalidadSeleccionada = null;
            }
            catch (Exception)
            {
                await _displayAlertService.MostrarAlertAsync("Error", "No se pudo registrar el cliente", "OK");
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
