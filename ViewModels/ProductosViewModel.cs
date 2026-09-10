using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Windows.Input;
using TradeFlow.Data.Repositories;
using TradeFlow.Models;
using TradeFlow.Services;
using TradeFlow.Views;

namespace TradeFlow.ViewModels
{
    public class ProductosViewModel : INotifyPropertyChanged
    {
        private readonly IProductoRepository _productoRepository;
        private readonly IImpresionService _impresionService;
        private readonly IDisplayAlertService _displayAlertService;

        private ObservableCollection<ProductoModel> _listaProductos = new ObservableCollection<ProductoModel>();
        private bool _isBusy;
        private string _textoBusqueda = string.Empty;
        private int _idBusquedaActual;
        private CancellationTokenSource? _searchCts;

        public ObservableCollection<ProductoModel> ListaProductos
        {
            get => _listaProductos;
            set
            {
                if (_listaProductos != value)
                {
                    _listaProductos = value;
                    OnPropertyChanged(nameof(ListaProductos));
                    OnPropertyChanged(nameof(TotalProductos));
                }
            }
        }

        public int TotalProductos => ListaProductos.Count;

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

        public string TextoBusqueda
        {
            get => _textoBusqueda;
            set
            {
                var nuevoValor = value?.Trim() ?? string.Empty;
                if (_textoBusqueda != nuevoValor)
                {
                    _textoBusqueda = nuevoValor;
                    OnPropertyChanged(nameof(TextoBusqueda));
                    _searchCts?.Cancel();
                    _searchCts = new CancellationTokenSource();
                    var token = _searchCts.Token;
                    _ = BuscarAsync(token);
                }
            }
        }

        public ICommand EliminarCommand { get; }
        public ICommand VerDetalleCommand { get; }
        public ICommand IrAAgregarCommand { get; }
        public ICommand ExportarPdfCommand { get; }

        public ProductosViewModel(IProductoRepository productoRepository, IImpresionService impresionService, IDisplayAlertService displayAlertService)
        {
            _productoRepository = productoRepository;
            _impresionService = impresionService;
            _displayAlertService = displayAlertService;
            EliminarCommand = new Command<ProductoModel>(async (producto) => await EliminarAsync(producto));
            VerDetalleCommand = new Command<ProductoModel>(async (producto) => await Shell.Current.GoToAsync($"{nameof(DetalleProductoView)}?productoId={producto.Id}"));
            IrAAgregarCommand = new Command(async () => await Shell.Current.GoToAsync(nameof(AgregarProductoView)));
            ExportarPdfCommand = new Command(async () => await ExportarPdfAsync());
        }

        public async Task InicializarAsync()
        {
            await BuscarAsync(CancellationToken.None);
        }

        public async Task BuscarAsync(CancellationToken ct = default)
        {
            var idBusqueda = ++_idBusquedaActual;

            try
            {
                IsBusy = true;

                await Task.Delay(300);
                if (ct.IsCancellationRequested || idBusqueda != _idBusquedaActual) return;

                var productos = string.IsNullOrWhiteSpace(TextoBusqueda)
                    ? await _productoRepository.ObtenerTodosAsync()
                    : await _productoRepository.BuscarAsync(TextoBusqueda.Trim());

                if (idBusqueda != _idBusquedaActual) return;

                _listaProductos = new ObservableCollection<ProductoModel>(productos);
                OnPropertyChanged(nameof(ListaProductos));
                OnPropertyChanged(nameof(TotalProductos));
            }
            catch (OperationCanceledException)
            {
            }
            catch (Exception)
            {
                if (idBusqueda != _idBusquedaActual) return;
                await _displayAlertService.MostrarAlertAsync("Error", "No se pudieron buscar los productos", "OK");
            }
            finally
            {
                if (idBusqueda == _idBusquedaActual)
                {
                    IsBusy = false;
                }
            }
        }

        public async Task EliminarAsync(ProductoModel producto)
        {
            try
            {
                var confirmar = await _displayAlertService.MostrarAlertConConfirmacionAsync(
                    "Eliminar", $"¿Eliminar el producto {producto.Nombre}?", "Eliminar", "Cancelar");

                if (confirmar)
                {
                    await _productoRepository.EliminarAsync(producto);
                    _listaProductos.Remove(producto);
                    OnPropertyChanged(nameof(TotalProductos));
                }
            }
            catch (Exception)
            {
                await _displayAlertService.MostrarAlertAsync("Error", "No se pudo eliminar el producto", "OK");
            }
        }

        public async Task CambiarEstadoAsync(ProductoModel producto)
        {
            try
            {
                await _productoRepository.GuardarAsync(producto);
            }
            catch (Exception)
            {
                await _displayAlertService.MostrarAlertAsync("Error", "No se pudo actualizar el estado del producto", "OK");
            }
        }

        private async Task ExportarPdfAsync()
        {
            try
            {
                IsBusy = true;

                var productos = await _productoRepository.ObtenerTodosAsync();
                if (productos == null || productos.Count == 0)
                {
                    await _displayAlertService.MostrarAlertAsync("Aviso", "No hay productos para exportar", "OK");
                    return;
                }

                var ruta = await _impresionService.GenerarCatalogoPdfAsync(productos);
                await _displayAlertService.MostrarAlertAsync("Éxito", $"Catálogo exportado correctamente\n{ruta}", "OK");
            }
            catch (Exception)
            {
                await _displayAlertService.MostrarAlertAsync("Error", "No se pudo exportar el catálogo a PDF", "OK");
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
