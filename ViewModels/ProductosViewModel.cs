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
    public class ProductosViewModel
    {
        private readonly IProductoRepository _productoRepository;
        private readonly IDisplayAlertService _displayAlertService;

        private ObservableCollection<ProductoModel> _listaProductos = new ObservableCollection<ProductoModel>();
        private bool _isBusy;
        private string _textoBusqueda = string.Empty;

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
                }
            }
        }

        public ICommand EliminarCommand { get; }
        public ICommand VerDetalleCommand { get; }
        public ICommand IrAAgregarCommand { get; }
        public ICommand BuscarCommand { get; }

        public ProductosViewModel(IProductoRepository productoRepository, IDisplayAlertService displayAlertService)
        {
            _productoRepository = productoRepository;
            _displayAlertService = displayAlertService;
            EliminarCommand = new Command<ProductoModel>(async (producto) => await EliminarAsync(producto));
            VerDetalleCommand = new Command<ProductoModel>(async (producto) => await Shell.Current.GoToAsync($"detalleproducto?productoId={producto.Id}"));
            IrAAgregarCommand = new Command(async () => await Shell.Current.GoToAsync(nameof(AgregarProductoView)));
            BuscarCommand = new Command(async () => await BuscarAsync());
        }

        public async Task InicializarAsync()
        {
            try
            {
                IsBusy = true;
                _listaProductos.Clear();

                var productos = await _productoRepository.ObtenerTodosAsync();
                foreach (var producto in productos)
                {
                    _listaProductos.Add(producto);
                }
            }
            catch (Exception)
            {
                await _displayAlertService.MostrarAlertAsync("Error", "No se pudieron cargar los productos", "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }

        public async Task BuscarAsync()
        {
            try
            {
                IsBusy = true;
                _listaProductos.Clear();

                var productos = string.IsNullOrWhiteSpace(TextoBusqueda)
                    ? await _productoRepository.ObtenerTodosAsync()
                    : await _productoRepository.BuscarPorNombreAsync(TextoBusqueda.Trim());

                foreach (var producto in productos)
                {
                    _listaProductos.Add(producto);
                }
            }
            catch (Exception)
            {
                await _displayAlertService.MostrarAlertAsync("Error", "No se pudieron buscar los productos", "OK");
            }
            finally
            {
                IsBusy = false;
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
                }
            }
            catch (Exception)
            {
                await _displayAlertService.MostrarAlertAsync("Error", "No se pudo eliminar el producto", "OK");
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
