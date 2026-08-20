using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using TradeFlow.Data.Repositories;
using TradeFlow.Models;
using TradeFlow.Services;

namespace TradeFlow.ViewModels
{
    [QueryProperty(nameof(ProductoId), "productoId")]
    public class EditarProductoViewModel
    {
        private readonly IProductoRepository _productoRepository;
        private readonly IDisplayAlertService _displayAlertService;
        private readonly IValidacionesService _validacionesService;

        private int _productoId;
        private ProductoModel? _producto;
        private string _nombre = string.Empty;
        private string _codigo = string.Empty;
        private decimal _precio;
        private bool _hayErrorEnNombre;
        private string _errorNombre = string.Empty;
        private bool _hayErrorEnCodigo;
        private string _errorCodigo = string.Empty;
        private bool _hayErrorEnPrecio;
        private string _errorPrecio = string.Empty;
        private bool _isBusy;

        public int ProductoId
        {
            get => _productoId;
            set
            {
                if (_productoId != value)
                {
                    _productoId = value;
                    OnPropertyChanged();
                }
            }
        }

        public ProductoModel? Producto
        {
            get => _producto;
            set
            {
                if (_producto != value)
                {
                    _producto = value;
                    OnPropertyChanged(nameof(Producto));
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

        public decimal Precio
        {
            get => _precio;
            set
            {
                if (_precio != value)
                {
                    _precio = value;
                    OnPropertyChanged(nameof(Precio));
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

        public ICommand GuardarCommand { get; }

        public EditarProductoViewModel(IProductoRepository productoRepository, IDisplayAlertService displayAlertService, IValidacionesService validacionesService)
        {
            _productoRepository = productoRepository;
            _displayAlertService = displayAlertService;
            _validacionesService = validacionesService;
            GuardarCommand = new Command(async () => await GuardarAsync());
        }

        public async Task InicializarAsync()
        {
            try
            {
                IsBusy = true;

                Producto = await _productoRepository.ObtenerPorIdAsync(ProductoId);
                if (Producto == null)
                {
                    await _displayAlertService.MostrarAlertAsync("Error", "No se encontró el producto", "OK");
                    return;
                }

                Nombre = Producto.Nombre;
                Codigo = Producto.Codigo;
                Precio = Producto.Precio;
            }
            catch (Exception)
            {
                await _displayAlertService.MostrarAlertAsync("Error", "No se pudo cargar el producto", "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }

        public async Task GuardarAsync()
        {
            if (Producto == null) return;

            try
            {
                IsBusy = true;

                ErrorNombre = _validacionesService.ValidarCampoVacio(Nombre);
                HayErrorEnNombre = !string.IsNullOrEmpty(ErrorNombre);

                ErrorCodigo = _validacionesService.ValidarCampoVacio(Codigo);
                HayErrorEnCodigo = !string.IsNullOrEmpty(ErrorCodigo);

                ErrorPrecio = _validacionesService.ValidarCampoVacio(Precio.ToString());
                HayErrorEnPrecio = !string.IsNullOrEmpty(ErrorPrecio);

                if (HayErrorEnNombre || HayErrorEnCodigo || HayErrorEnPrecio)
                {
                    return;
                }

                Producto.Nombre = Nombre;
                Producto.Codigo = Codigo;
                Producto.Precio = Precio;

                await _productoRepository.GuardarAsync(Producto);

                await _displayAlertService.MostrarAlertAsync("Éxito", "Producto actualizado correctamente", "OK");

                await Shell.Current.GoToAsync("..");
            }
            catch (Exception)
            {
                await _displayAlertService.MostrarAlertAsync("Error", "No se pudo actualizar el producto", "OK");
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
