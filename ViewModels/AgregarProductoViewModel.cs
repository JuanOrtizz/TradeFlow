using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using TradeFlow.Data.Repositories;
using TradeFlow.Services;
using TradeFlow.Views;

namespace TradeFlow.ViewModels
{
    public class AgregarProductoViewModel : INotifyPropertyChanged
    {
        private readonly IProductoRepository _productoRepository;
        private readonly IDisplayAlertService _displayAlertService;
        private readonly IValidacionesService _validacionesService;

        private string _nombre = string.Empty;
        private string _codigo = string.Empty;
        private string _precioTexto = string.Empty;
        private bool _hayErrorEnNombre;
        private string _errorNombre = string.Empty;
        private bool _hayErrorEnCodigo;
        private string _errorCodigo = string.Empty;
        private bool _hayErrorEnPrecio;
        private string _errorPrecio = string.Empty;
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

        public string Codigo
        {
            get => _codigo;
            set
            {
                var nuevoValor = value?.Trim() ?? string.Empty;
                if (_codigo != nuevoValor)
                {
                    _codigo = nuevoValor;
                    OnPropertyChanged(nameof(Codigo));
                }
            }
        }

        public string PrecioTexto
        {
            get => _precioTexto;
            set
            {
                var nuevoValor = value?.Trim() ?? string.Empty;
                if (_precioTexto != nuevoValor)
                {
                    _precioTexto = nuevoValor;
                    OnPropertyChanged(nameof(PrecioTexto));
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

        public bool HayErrorEnCodigo
        {
            get => _hayErrorEnCodigo;
            set
            {
                if (_hayErrorEnCodigo != value)
                {
                    _hayErrorEnCodigo = value;
                    OnPropertyChanged();
                }
            }
        }

        public string ErrorCodigo
        {
            get => _errorCodigo;
            set
            {
                var nuevoValor = value?.Trim() ?? string.Empty;
                if (_errorCodigo != nuevoValor)
                {
                    _errorCodigo = nuevoValor;
                    OnPropertyChanged(nameof(ErrorCodigo));
                }
            }
        }

        public bool HayErrorEnPrecio
        {
            get => _hayErrorEnPrecio;
            set
            {
                if (_hayErrorEnPrecio != value)
                {
                    _hayErrorEnPrecio = value;
                    OnPropertyChanged();
                }
            }
        }

        public string ErrorPrecio
        {
            get => _errorPrecio;
            set
            {
                var nuevoValor = value?.Trim() ?? string.Empty;
                if (_errorPrecio != nuevoValor)
                {
                    _errorPrecio = nuevoValor;
                    OnPropertyChanged(nameof(ErrorPrecio));
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

        public ICommand RegistroProductoCommand { get; }
        public ICommand VolverCommand { get; }
        public ICommand LimpiarErrorCommand { get; }

        public AgregarProductoViewModel( IProductoRepository productoRepository, IDisplayAlertService displayAlertService, IValidacionesService validacionesService)
        {
            _productoRepository = productoRepository;
            _displayAlertService = displayAlertService;
            _validacionesService = validacionesService;
            RegistroProductoCommand = new Command(async () => await GuardarAsync());
            VolverCommand = new Command(async () => await Shell.Current.GoToAsync(".."));
            LimpiarErrorCommand = new Command<string>(LimpiarError);
        }

        private void LimpiarError(string campo)
        {
            switch (campo)
            {
                case "Nombre":
                    HayErrorEnNombre = false;
                    ErrorNombre = string.Empty;
                    break;
                case "Codigo":
                    HayErrorEnCodigo = false;
                    ErrorCodigo = string.Empty;
                    break;
                case "Precio":
                    HayErrorEnPrecio = false;
                    ErrorPrecio = string.Empty;
                    break;
            }
        }

        public async Task GuardarAsync()
        {
            ErrorNombre = _validacionesService.ValidarCampoVacio(Nombre);
            HayErrorEnNombre = !string.IsNullOrEmpty(ErrorNombre);

            decimal precio = 0;

            ErrorPrecio = _validacionesService.ValidarCampoVacio(PrecioTexto);
            if (string.IsNullOrEmpty(ErrorPrecio))
            {
                if (!_validacionesService.TryObtenerDecimal(PrecioTexto, out precio))
                {
                    ErrorPrecio = "El precio debe ser un número válido.";
                }
                else
                {
                    ErrorPrecio = _validacionesService.ValidarPrecio(precio);
                }
            }
            HayErrorEnPrecio = !string.IsNullOrEmpty(ErrorPrecio);

            if (HayErrorEnNombre || HayErrorEnCodigo || HayErrorEnPrecio)
            {
                return;
            }

            try
            {
                IsBusy = true;

                // Validar duplicados
                if (await _productoRepository.ExisteNombreAsync(Nombre))
                {
                    ErrorNombre = "Ya existe un producto registrado con este nombre";
                    HayErrorEnNombre = true;
                    return;
                }

                if (!string.IsNullOrEmpty(Codigo) && await _productoRepository.ExisteCodigoAsync(Codigo))
                {
                    ErrorCodigo = "Ya existe un producto registrado con este código";
                    HayErrorEnCodigo = true;
                    return;
                }

                var producto = await _productoRepository.RegistrarAsync(Nombre, Codigo, precio);

                await _displayAlertService.MostrarAlertAsync("Éxito", "Producto registrado correctamente", "OK");

                await Shell.Current.GoToAsync($"{nameof(DetalleProductoView)}?productoId={producto.Id}");

                var navigation = Shell.Current.Navigation;
                if (navigation.NavigationStack.Count > 2)
                {
                    navigation.RemovePage(navigation.NavigationStack[navigation.NavigationStack.Count - 2]);
                }
            }
            catch (Exception)
            {
                await _displayAlertService.MostrarAlertAsync("Error", "No se pudo registrar el producto", "OK");
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
