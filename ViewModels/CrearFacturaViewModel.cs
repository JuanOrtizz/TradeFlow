using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using TradeFlow.Data.Repositories;
using TradeFlow.Models;
using TradeFlow.Services;
using TradeFlow.Views;

namespace TradeFlow.ViewModels
{
    public class CrearFacturaViewModel : INotifyPropertyChanged
    {
        private readonly IFacturaRepository _facturaRepository;
        private readonly IClienteRepository _clienteRepository;
        private readonly IProductoRepository _productoRepository;
        private readonly IDisplayAlertService _displayAlertService;
        private readonly IValidacionesService _validacionesService;

        private ClienteModel? _clienteSeleccionado;
        private ProductoModel? _productoSeleccionado;
        private int _cantidad = 1;
        private int _descuentoPorcentaje;
        private int _indiceDescuento;
        private decimal _subtotalItem;
        private bool _isBusy;
        private string _textoBuscarCliente = string.Empty;
        private string _textoBuscarProducto = string.Empty;
        private bool _haySugerenciasClientes;
        private bool _haySugerenciasProductos;

        private bool _hayErrorEnCliente;
        private string _errorCliente = string.Empty;
        private bool _hayErrorEnProducto;
        private string _errorProducto = string.Empty;

        public ObservableCollection<ClienteModel> ListaClientes { get; } = new ObservableCollection<ClienteModel>();
        public ObservableCollection<ProductoModel> ListaProductos { get; } = new ObservableCollection<ProductoModel>();
        public ObservableCollection<DetalleFacturaModel> ItemsFactura { get; } = new ObservableCollection<DetalleFacturaModel>();
        public ObservableCollection<ClienteModel> SugerenciasClientes { get; } = new ObservableCollection<ClienteModel>();
        public ObservableCollection<ProductoModel> SugerenciasProductos { get; } = new ObservableCollection<ProductoModel>();

        public IReadOnlyList<string> OpcionesDescuento { get; } =
            new List<string> { "Sin Descuento" }
                .Concat(Enumerable.Range(1, 10).Select(i => $"{i}%"))
                .ToList();

        public ClienteModel? ClienteSeleccionado
        {
            get => _clienteSeleccionado;
            set
            {
                if (_clienteSeleccionado != value)
                {
                    _clienteSeleccionado = value;
                    OnPropertyChanged(nameof(ClienteSeleccionado));
                    OnPropertyChanged(nameof(TieneClienteSeleccionado));
                }
            }
        }

        public bool TieneClienteSeleccionado => ClienteSeleccionado != null;

        public string TextoBuscarCliente
        {
            get => _textoBuscarCliente;
            set
            {
                var nuevoValor = value ?? string.Empty;
                if (_textoBuscarCliente != nuevoValor)
                {
                    _textoBuscarCliente = nuevoValor;
                    OnPropertyChanged(nameof(TextoBuscarCliente));
                    BuscarClientes();
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

        public bool HayErrorEnCliente
        {
            get => _hayErrorEnCliente;
            set { if (_hayErrorEnCliente != value) { _hayErrorEnCliente = value; OnPropertyChanged(); } }
        }

        public string ErrorCliente
        {
            get => _errorCliente;
            set { if (_errorCliente != value) { _errorCliente = value; OnPropertyChanged(); } }
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

        public bool HaySugerenciasClientes
        {
            get => _haySugerenciasClientes;
            set { if (_haySugerenciasClientes != value) { _haySugerenciasClientes = value; OnPropertyChanged(); } }
        }

        public bool HaySugerenciasProductos
        {
            get => _haySugerenciasProductos;
            set { if (_haySugerenciasProductos != value) { _haySugerenciasProductos = value; OnPropertyChanged(); } }
        }

        public ICommand AgregarItemCommand { get; }
        public ICommand EliminarItemCommand { get; }
        public ICommand RegistrarFacturaCommand { get; }
        public ICommand VolverCommand { get; }
        public ICommand LimpiarErrorCommand { get; }
        public ICommand SeleccionarClienteCommand { get; }
        public ICommand DeseleccionarClienteCommand { get; }
        public ICommand SeleccionarProductoCommand { get; }
        public ICommand DeseleccionarProductoCommand { get; }
        public ICommand MasCantidadCommand { get; }
        public ICommand MenosCantidadCommand { get; }
        public ICommand OcultarSugerenciasClienteCommand { get; }
        public ICommand OcultarSugerenciasProductoCommand { get; }

        public CrearFacturaViewModel(
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

            AgregarItemCommand = new Command(async () => await AgregarItemAsync());
            EliminarItemCommand = new Command<DetalleFacturaModel>(async (item) => await EliminarItemAsync(item));
            RegistrarFacturaCommand = new Command(async () => await RegistrarFacturaAsync());
            VolverCommand = new Command(async () => await Shell.Current.GoToAsync(".."));
            LimpiarErrorCommand = new Command<string>(LimpiarError);
            SeleccionarClienteCommand = new Command<ClienteModel>(SeleccionarCliente);
            DeseleccionarClienteCommand = new Command(DeseleccionarCliente);
            SeleccionarProductoCommand = new Command<ProductoModel>(SeleccionarProducto);
            DeseleccionarProductoCommand = new Command(DeseleccionarProducto);
            MasCantidadCommand = new Command<DetalleFacturaModel>(item => CambiarCantidad(item, 1));
            MenosCantidadCommand = new Command<DetalleFacturaModel>(item => CambiarCantidad(item, -1));
            OcultarSugerenciasClienteCommand = new Command(async () => await OcultarSugerenciasClienteAsync());
            OcultarSugerenciasProductoCommand = new Command(async () => await OcultarSugerenciasProductoAsync());
        }

        private void LimpiarError(string campo)
        {
            switch (campo)
            {
                case "Cliente":
                    HayErrorEnCliente = false;
                    ErrorCliente = string.Empty;
                    break;
                case "Producto":
                    HayErrorEnProducto = false;
                    ErrorProducto = string.Empty;
                    break;
            }
        }

        private void BuscarClientes()
        {
            var texto = _textoBuscarCliente.Trim();

            SugerenciasClientes.Clear();
            if (texto.Length == 0)
            {
                HaySugerenciasClientes = false;
                return;
            }

            var coincidencias = ListaClientes
                .Where(c => c.Nombre.IndexOf(texto, StringComparison.OrdinalIgnoreCase) >= 0)
                .Take(8);

            foreach (var c in coincidencias) SugerenciasClientes.Add(c);
            HaySugerenciasClientes = SugerenciasClientes.Count > 0;
        }

        private void BuscarProductos()
        {
            var texto = _textoBuscarProducto.Trim();

            SugerenciasProductos.Clear();
            if (texto.Length == 0)
            {
                HaySugerenciasProductos = false;
                return;
            }

            var coincidencias = ListaProductos
                .Where(p => p.Nombre.IndexOf(texto, StringComparison.OrdinalIgnoreCase) >= 0
                         || (p.Codigo ?? string.Empty).IndexOf(texto, StringComparison.OrdinalIgnoreCase) >= 0)
                .Take(8);

            foreach (var p in coincidencias) SugerenciasProductos.Add(p);
            HaySugerenciasProductos = SugerenciasProductos.Count > 0;
        }

        public async Task OcultarSugerenciasClienteAsync()
        {
            await Task.Delay(150);
            SugerenciasClientes.Clear();
            HaySugerenciasClientes = false;
        }

        public async Task OcultarSugerenciasProductoAsync()
        {
            await Task.Delay(150);
            SugerenciasProductos.Clear();
            HaySugerenciasProductos = false;
        }

        private void SeleccionarCliente(ClienteModel? cliente)
        {
            if (cliente == null) return;

            ClienteSeleccionado = cliente;
            TextoBuscarCliente = string.Empty;
            SugerenciasClientes.Clear();
            HaySugerenciasClientes = false;
            HayErrorEnCliente = false;
            ErrorCliente = string.Empty;
        }

        private void DeseleccionarCliente()
        {
            ClienteSeleccionado = null;
            TextoBuscarCliente = string.Empty;
            SugerenciasClientes.Clear();
            HaySugerenciasClientes = false;
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

                ListaClientes.Clear();
                var clientes = await _clienteRepository.ObtenerTodosAsync();
                foreach (var c in clientes) ListaClientes.Add(c);

                ListaProductos.Clear();
                var productos = await _productoRepository.ObtenerTodosAsync();
                foreach (var p in productos) ListaProductos.Add(p);

                ItemsFactura.Clear();
            }
            catch (Exception)
            {
                await _displayAlertService.MostrarAlertAsync("Error", "No se pudieron cargar los datos", "OK");
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

        public async Task AgregarItemAsync()
        {
            ErrorCliente = _validacionesService.ValidarSeleccion(ClienteSeleccionado, "cliente");
            HayErrorEnCliente = !string.IsNullOrEmpty(ErrorCliente);

            ErrorProducto = _validacionesService.ValidarSeleccion(ProductoSeleccionado, "producto");
            HayErrorEnProducto = !string.IsNullOrEmpty(ErrorProducto);

            if (HayErrorEnCliente || HayErrorEnProducto) return;

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

            ItemsFactura.Add(item);
            OnPropertyChanged(nameof(Total));

            DeseleccionarProducto();
            Cantidad = 1;
            IndiceDescuento = 0;
        }

        private void CambiarCantidad(DetalleFacturaModel? item, int delta)
        {
            if (item == null) return;

            var nuevaCantidad = item.Cantidad + delta;
            if (nuevaCantidad < 1) return;

            item.PrecioFinal = item.PrecioUnitario - (item.PrecioUnitario * item.DescuentoPorcentaje / 100m);
            item.Subtotal = item.PrecioFinal * nuevaCantidad;
            item.Cantidad = nuevaCantidad;

            OnPropertyChanged(nameof(Total));
        }

        public async Task EliminarItemAsync(DetalleFacturaModel item)
        {
            if (item == null) return;
            ItemsFactura.Remove(item);
            OnPropertyChanged(nameof(Total));
        }

        public async Task RegistrarFacturaAsync()
        {
            if (ClienteSeleccionado == null)
            {
                await _displayAlertService.MostrarAlertAsync("Error", "Debe seleccionar un cliente", "OK");
                return;
            }

            if (ItemsFactura.Count == 0)
            {
                await _displayAlertService.MostrarAlertAsync("Error", "Debe agregar al menos un item", "OK");
                return;
            }

            try
            {
                IsBusy = true;
                var nuevaFactura = await _facturaRepository.RegistrarAsync(ClienteSeleccionado, ItemsFactura.ToList());
                await _displayAlertService.MostrarAlertAsync("Exito", "Factura registrada correctamente", "OK");

                await Shell.Current.GoToAsync($"{nameof(DetalleFacturaView)}?facturaId={nuevaFactura.Id}");

                var navigation = Shell.Current.Navigation;
                if (navigation.NavigationStack.Count > 2)
                {
                    navigation.RemovePage(navigation.NavigationStack[navigation.NavigationStack.Count - 2]);
                }
            }
            catch (Exception)
            {
                await _displayAlertService.MostrarAlertAsync("Error", "No se pudo registrar la factura", "OK");
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
