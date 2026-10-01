using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using TradeFlow.Data.Repositories;
using TradeFlow.Helpers;
using TradeFlow.Models;
using TradeFlow.Services;

namespace TradeFlow.ViewModels
{
    [QueryProperty(nameof(FacturaId), "facturaId")]
    public class EditarFacturaViewModel : INotifyPropertyChanged
    {
        private readonly IFacturaRepository _facturaRepository;
        private readonly IClienteRepository _clienteRepository;
        private readonly IProductoRepository _productoRepository;
        private readonly IDisplayAlertService _displayAlertService;
        private readonly IValidacionesService _validacionesService;

        public const int MaxItemsFactura = CrearFacturaViewModel.MaxItemsFactura;

        private int _facturaId;
        private FacturaModel? _factura;
        private ProductoModel? _productoSeleccionado;
        private int _cantidad = 1;
        private int _descuentoPorcentaje;
        private int _indiceDescuento;
        private decimal _subtotalItem;
        private bool _isBusy;
        private string _textoBuscarProducto = string.Empty;
        private bool _haySugerenciasProductos;
        private bool _hayErrorEnProducto;
        private string _errorProducto = string.Empty;

        private ObservableCollection<ProductoModel> _listaProductos = new ObservableCollection<ProductoModel>();
        private ObservableCollection<ProductoModel> _sugerenciasProductos = new ObservableCollection<ProductoModel>();

        public ObservableCollection<DetalleFacturaModel> ItemsFactura { get; } = new ObservableCollection<DetalleFacturaModel>();

        public IReadOnlyList<string> OpcionesDescuento { get; } =
            new List<string> { "Sin Descuento" }
                .Concat(Enumerable.Range(1, 10).Select(i => $"{i}%"))
                .ToList();

        public int FacturaId
        {
            get => _facturaId;
            set
            {
                if (_facturaId != value)
                {
                    _facturaId = value;
                    OnPropertyChanged();
                }
            }
        }

        public FacturaModel? Factura
        {
            get => _factura;
            set
            {
                if (_factura != value)
                {
                    _factura = value;
                    OnPropertyChanged(nameof(Factura));
                }
            }
        }

        public ObservableCollection<ProductoModel> ListaProductos
        {
            get => _listaProductos;
            set
            {
                if (_listaProductos != value)
                {
                    _listaProductos = value;
                    OnPropertyChanged(nameof(ListaProductos));
                }
            }
        }

        public ObservableCollection<ProductoModel> SugerenciasProductos
        {
            get => _sugerenciasProductos;
            set
            {
                if (_sugerenciasProductos != value)
                {
                    _sugerenciasProductos = value;
                    OnPropertyChanged(nameof(SugerenciasProductos));
                }
            }
        }

        public ProductoModel? ProductoSeleccionado
        {
            get => _productoSeleccionado;
            set
            {
                if (_productoSeleccionado != value)
                {
                    _productoSeleccionado = value;
                    OnPropertyChanged(nameof(ProductoSeleccionado));
                    OnPropertyChanged(nameof(TieneProductoSeleccionado));
                    CalcularSubtotal();
                }
            }
        }

        public bool TieneProductoSeleccionado => ProductoSeleccionado != null;

        public string TextoBuscarProducto
        {
            get => _textoBuscarProducto;
            set
            {
                var nuevoValor = value ?? string.Empty;
                if (_textoBuscarProducto != nuevoValor)
                {
                    _textoBuscarProducto = nuevoValor;
                    OnPropertyChanged(nameof(TextoBuscarProducto));
                    BuscarProductos();
                }
            }
        }

        public int Cantidad
        {
            get => _cantidad;
            set
            {
                if (_cantidad != value)
                {
                    _cantidad = value;
                    OnPropertyChanged(nameof(Cantidad));
                    CalcularSubtotal();
                }
            }
        }

        public int IndiceDescuento
        {
            get => _indiceDescuento;
            set
            {
                if (_indiceDescuento != value)
                {
                    _indiceDescuento = value;
                    OnPropertyChanged(nameof(IndiceDescuento));
                    DescuentoPorcentaje = value;
                }
            }
        }

        public int DescuentoPorcentaje
        {
            get => _descuentoPorcentaje;
            set
            {
                if (_descuentoPorcentaje != value)
                {
                    _descuentoPorcentaje = value;
                    OnPropertyChanged(nameof(DescuentoPorcentaje));
                    CalcularSubtotal();
                }
            }
        }

        public decimal SubtotalItem
        {
            get => _subtotalItem;
            set
            {
                if (_subtotalItem != value)
                {
                    _subtotalItem = value;
                    OnPropertyChanged(nameof(SubtotalItem));
                }
            }
        }

        public decimal Total => ItemsFactura.Sum(i => i.Subtotal);

        public string ContadorItems => $"{ItemsFactura.Count} / {MaxItemsFactura}";

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

        public bool HaySugerenciasProductos
        {
            get => _haySugerenciasProductos;
            set
            {
                if (_haySugerenciasProductos != value)
                {
                    _haySugerenciasProductos = value;
                    OnPropertyChanged(nameof(HaySugerenciasProductos));
                }
            }
        }

        public bool HayErrorEnProducto
        {
            get => _hayErrorEnProducto;
            set { if (_hayErrorEnProducto != value) { _hayErrorEnProducto = value; OnPropertyChanged(); } }
        }

        public string ErrorProducto
        {
            get => _errorProducto;
            set { if (_errorProducto != value) { _errorProducto = value; OnPropertyChanged(); } }
        }

        public ICommand GuardarCommand { get; }
        public ICommand VolverCommand { get; }
        public ICommand AgregarItemCommand { get; }
        public ICommand EliminarItemCommand { get; }
        public ICommand MasCantidadCommand { get; }
        public ICommand MenosCantidadCommand { get; }
        public ICommand SeleccionarProductoCommand { get; }
        public ICommand DeseleccionarProductoCommand { get; }
        public ICommand OcultarSugerenciasProductoCommand { get; }
        public ICommand LimpiarErrorCommand { get; }

        public EditarFacturaViewModel(
            IFacturaRepository facturaRepository,
            IClienteRepository clienteRepository,
            IProductoRepository productoRepository,
            IDisplayAlertService displayAlertService,
            IValidacionesService validacionesService)
        {
            _facturaRepository = facturaRepository;
            _clienteRepository = clienteRepository;
            _productoRepository = productoRepository;
            _displayAlertService = displayAlertService;
            _validacionesService = validacionesService;

            GuardarCommand = new Command(async () => await GuardarAsync());
            VolverCommand = new Command(async () => await Shell.Current.GoToAsync(".."));
            AgregarItemCommand = new Command(async () => await AgregarItemAsync());
            EliminarItemCommand = new Command<DetalleFacturaModel>(async (item) => await EliminarItemAsync(item));
            MasCantidadCommand = new Command<DetalleFacturaModel>(item => CambiarCantidad(item, 1));
            MenosCantidadCommand = new Command<DetalleFacturaModel>(item => CambiarCantidad(item, -1));
            SeleccionarProductoCommand = new Command<ProductoModel>(SeleccionarProducto);
            DeseleccionarProductoCommand = new Command(DeseleccionarProducto);
            OcultarSugerenciasProductoCommand = new Command(async () => await OcultarSugerenciasProductoAsync());
            LimpiarErrorCommand = new Command<string>(LimpiarError);
        }

        private void LimpiarError(string campo)
        {
            if (campo != "Producto") return;

            HayErrorEnProducto = false;
            ErrorProducto = string.Empty;
        }

        private void BuscarProductos()
        {
            var texto = _textoBuscarProducto.Trim();

            if (texto.Length == 0)
            {
                SugerenciasProductos = new ObservableCollection<ProductoModel>();
                HaySugerenciasProductos = false;
                return;
            }

            var coincidencias = BusquedaHelper
                .OrdenarPorRelevancia(
                    ListaProductos.Where(p => p.Nombre.IndexOf(texto, StringComparison.OrdinalIgnoreCase) >= 0
                                          || (p.Codigo ?? string.Empty).IndexOf(texto, StringComparison.OrdinalIgnoreCase) >= 0),
                    texto,
                    p => p.Nombre,
                    p => p.Codigo)
                .Take(8)
                .ToList();

            SugerenciasProductos = new ObservableCollection<ProductoModel>(coincidencias);
            HaySugerenciasProductos = coincidencias.Count > 0;
        }

        public async Task OcultarSugerenciasProductoAsync()
        {
            await Task.Delay(150);
            SugerenciasProductos.Clear();
            HaySugerenciasProductos = false;
        }

        private void SeleccionarProducto(ProductoModel? producto)
        {
            if (producto == null) return;

            ProductoSeleccionado = producto;
            TextoBuscarProducto = string.Empty;
            SugerenciasProductos.Clear();
            HaySugerenciasProductos = false;
            HayErrorEnProducto = false;
            ErrorProducto = string.Empty;
        }

        private void DeseleccionarProducto()
        {
            ProductoSeleccionado = null;
            TextoBuscarProducto = string.Empty;
            SugerenciasProductos.Clear();
            HaySugerenciasProductos = false;
        }

        public async Task InicializarAsync()
        {
            try
            {
                IsBusy = true;

                var factura = await _facturaRepository.ObtenerPorIdAsync(FacturaId);
                if (factura == null)
                {
                    await _displayAlertService.MostrarAlertAsync("Error", "No se pudo cargar la factura", "OK");
                    return;
                }

                factura.Cliente = await _clienteRepository.ObtenerPorIdAsync(factura.ClienteId);
                Factura = factura;

                foreach (var item in ItemsFactura.ToList())
                {
                    DejarDeVigilar(item);
                }

                ItemsFactura.Clear();
                foreach (var detalle in await _facturaRepository.ObtenerDetallesAsync(FacturaId))
                {
                    Vigilar(detalle);
                    ItemsFactura.Add(detalle);
                }
                OnPropertyChanged(nameof(ContadorItems));
                OnPropertyChanged(nameof(Total));

                ListaProductos = new ObservableCollection<ProductoModel>(await _productoRepository.ObtenerActivosAsync());
            }
            catch (Exception)
            {
                await _displayAlertService.MostrarAlertAsync("Error", "No se pudo cargar la factura", "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }

        private void CalcularSubtotal()
        {
            if (ProductoSeleccionado == null || Cantidad <= 0) { SubtotalItem = 0; return; }

            var precio = ProductoSeleccionado.Precio * Cantidad;
            var descuento = precio * DescuentoPorcentaje / 100m;
            SubtotalItem = precio - descuento;
        }

        private async Task AgregarItemAsync()
        {
            ErrorProducto = _validacionesService.ValidarSeleccion(ProductoSeleccionado, "producto");
            HayErrorEnProducto = !string.IsNullOrEmpty(ErrorProducto);

            if (HayErrorEnProducto) return;

            if (Cantidad <= 0)
            {
                await _displayAlertService.MostrarAlertAsync("Error", "La cantidad debe ser mayor a 0", "OK");
                return;
            }

            if (ItemsFactura.Any(i => i.ProductoId == ProductoSeleccionado!.Id))
            {
                await _displayAlertService.MostrarAlertAsync("Producto duplicado", "El producto ya está agregado a la factura", "OK");
                return;
            }

            if (ItemsFactura.Count >= MaxItemsFactura)
            {
                await _displayAlertService.MostrarAlertAsync("Limite alcanzado", $"Una boleta admite hasta {MaxItemsFactura} productos", "OK");
                return;
            }

            CalcularSubtotal();

            var item = new DetalleFacturaModel
            {
                ProductoId = ProductoSeleccionado!.Id,
                ProductoNombre = ProductoSeleccionado.Nombre,
                Codigo = ProductoSeleccionado.Codigo,
                Cantidad = Cantidad,
                PrecioUnitario = ProductoSeleccionado.Precio,
                DescuentoPorcentaje = DescuentoPorcentaje,
                PrecioFinal = ProductoSeleccionado.Precio - (ProductoSeleccionado.Precio * DescuentoPorcentaje / 100m),
                Subtotal = SubtotalItem
            };

            Vigilar(item);
            ItemsFactura.Add(item);
            OnPropertyChanged(nameof(Total));
            OnPropertyChanged(nameof(ContadorItems));

            DeseleccionarProducto();
            Cantidad = 1;
            IndiceDescuento = 0;
        }

        private void CambiarCantidad(DetalleFacturaModel? item, int delta)
        {
            if (item == null) return;

            var nuevaCantidad = item.Cantidad + delta;
            if (nuevaCantidad < 1) return;

            item.Cantidad = nuevaCantidad;

            OnPropertyChanged(nameof(Total));
        }

        private async Task EliminarItemAsync(DetalleFacturaModel item)
        {
            if (item == null) return;

            var confirmar = await _displayAlertService.MostrarAlertConConfirmacionAsync(
                "Eliminar", $"¿Quitar {item.ProductoNombre} de la factura?", "Eliminar", "Cancelar");

            if (!confirmar) return;

            DejarDeVigilar(item);
            ItemsFactura.Remove(item);
            OnPropertyChanged(nameof(Total));
            OnPropertyChanged(nameof(ContadorItems));
        }

        private async Task GuardarAsync()
        {
            if (Factura == null) return;

            if (ItemsFactura.Count == 0)
            {
                await _displayAlertService.MostrarAlertAsync("Error", "La factura debe tener al menos un producto", "OK");
                return;
            }

            try
            {
                IsBusy = true;

                foreach (var item in ItemsFactura)
                {
                    DejarDeVigilar(item);
                }

                await _facturaRepository.ActualizarAsync(Factura, ItemsFactura.ToList());

                foreach (var item in ItemsFactura)
                {
                    Vigilar(item);
                }

                await _displayAlertService.MostrarAlertAsync("Exito", "Factura actualizada correctamente", "OK");
                await Shell.Current.GoToAsync("..");
            }
            catch (Exception)
            {
                await _displayAlertService.MostrarAlertAsync("Error", "No se pudo actualizar la factura", "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }

        private void Vigilar(DetalleFacturaModel item)
        {
            item.PropertyChanged += AlCambiarPropiedadDelItem;
        }

        private void DejarDeVigilar(DetalleFacturaModel item)
        {
            item.PropertyChanged -= AlCambiarPropiedadDelItem;
        }

        private void AlCambiarPropiedadDelItem(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(DetalleFacturaModel.Subtotal))
            {
                OnPropertyChanged(nameof(Total));
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
