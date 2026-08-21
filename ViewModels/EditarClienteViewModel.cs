using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using TradeFlow.Data.Repositories;
using TradeFlow.Models;
using TradeFlow.Services;

namespace TradeFlow.ViewModels
{
    [QueryProperty(nameof(ClienteId), "clienteId")]
    public class EditarClienteViewModel
    {
        private readonly IClienteRepository _clienteRepository;
        private readonly ILocalidadRepository _localidadRepository;
        private readonly IDisplayAlertService _displayAlertService;
        private readonly IValidacionesService _validacionesService;

        private int _clienteId;
        private ClienteModel? _cliente;
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

        public int ClienteId
        {
            get => _clienteId;
            set
            {
                if (_clienteId != value)
                {
                    _clienteId = value;
                    OnPropertyChanged();
                }
            }
        }

        public ClienteModel? Cliente
        {
            get => _cliente;
            set
            {
                if (_cliente != value)
                {
                    _cliente = value;
                    OnPropertyChanged(nameof(Cliente));
                }
            }
        }

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

        public ICommand GuardarCommand { get; }

        public EditarClienteViewModel(
            IClienteRepository clienteRepository,
            ILocalidadRepository localidadRepository,
            IDisplayAlertService displayAlertService,
            IValidacionesService validacionesService)
        {
            _clienteRepository = clienteRepository;
            _localidadRepository = localidadRepository;
            _displayAlertService = displayAlertService;
            _validacionesService = validacionesService;
            GuardarCommand = new Command(async () => await GuardarAsync());
        }

        public async Task InicializarAsync()
        {
            try
            {
                IsBusy = true;

                Cliente = await _clienteRepository.ObtenerPorIdAsync(ClienteId);
                if (Cliente == null)
                {
                    await _displayAlertService.MostrarAlertAsync("Error", "No se encontró el cliente", "OK");
                    return;
                }

                Nombre = Cliente.Nombre;
                Telefono = Cliente.Telefono;
                Direccion = Cliente.Direccion;

                ListaLocalidades.Clear();
                var localidades = await _localidadRepository.ObtenerTodasAsync();
                foreach (var localidad in localidades)
                {
                    ListaLocalidades.Add(localidad);
                }

                LocalidadSeleccionada = ListaLocalidades.FirstOrDefault(l => l.Id == Cliente.LocalidadId);
            }
            catch (Exception)
            {
                await _displayAlertService.MostrarAlertAsync("Error", "No se pudo cargar el cliente", "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }

        public async Task GuardarAsync()
        {
            if (Cliente == null) return;

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

                Cliente.Nombre = Nombre;
                Cliente.Telefono = Telefono;
                Cliente.Direccion = Direccion;
                Cliente.LocalidadId = LocalidadSeleccionada.Id;

                await _clienteRepository.GuardarAsync(Cliente);

                await _displayAlertService.MostrarAlertAsync("Éxito", "Cliente actualizado correctamente", "OK");

                await Shell.Current.GoToAsync("..");
            }
            catch (Exception)
            {
                await _displayAlertService.MostrarAlertAsync("Error", "No se pudo actualizar el cliente", "OK");
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
