using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using TradeFlow.Data.Repositories;
using TradeFlow.Models;
using TradeFlow.Services;

namespace TradeFlow.ViewModels
{
    [QueryProperty(nameof(ProductoId), "productoId")]
    public class EditarProductoViewModel : INotifyPropertyChanged
    {
        private readonly IProductoRepository _productoRepository;
        private readonly IDisplayAlertService _displayAlertService;
        private readonly IValidacionesService _validacionesService;

        private int _productoId;
        private ProductoModel? _producto;
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

        public ICommand GuardarCommand { get; }
        public ICommand VolverCommand { get; }
        public ICommand LimpiarErrorCommand { get; }

        public EditarProductoViewModel(IProductoRepository productoRepository, IDisplayAlertService displayAlertService, IValidacionesService validacionesService)
        {
            _productoRepository = productoRepository;
            _displayAlertService = displayAlertService;
            _validacionesService = validacionesService;
            GuardarCommand = new Command(async () => await GuardarAsync());
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
                PrecioTexto = Producto.Precio.ToString();
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

                // Validar duplicados (excluyo el propio registro)
                if (await _productoRepository.ExisteNombreAsync(Nombre, Producto.Id))
                {
                    ErrorNombre = "Ya existe un producto registrado con este nombre";
                    HayErrorEnNombre = true;
                    return;
                }

                if (!string.IsNullOrEmpty(Codigo) && await _productoRepository.ExisteCodigoAsync(Codigo, Producto.Id))
                {
                    ErrorCodigo = "Ya existe un producto registrado con este código";
                    HayErrorEnCodigo = true;
                    return;
                }

                Producto.Nombre = Nombre;
                Producto.Codigo = Codigo;
                Producto.Precio = precio;

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
