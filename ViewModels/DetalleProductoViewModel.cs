using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using TradeFlow.Data.Repositories;
using TradeFlow.Models;
using TradeFlow.Services;
using TradeFlow.Views;

namespace TradeFlow.ViewModels
{
    [QueryProperty(nameof(ProductoId), "productoId")]
    public class DetalleProductoViewModel : INotifyPropertyChanged
    {
        private readonly IProductoRepository _productoRepository;
        private readonly IDisplayAlertService _displayAlertService;

        private int _productoId;
        private ProductoModel? _producto;
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
                    OnPropertyChanged(nameof(EstadoTexto));
                }
            }
        }

        public string EstadoTexto => Producto?.Activo == true ? "Activo" : "Inactivo";

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

        public ICommand EditarCommand { get; }
        public ICommand EliminarCommand { get; }
        public ICommand VolverCommand { get; }

        public DetalleProductoViewModel(IProductoRepository productoRepository, IDisplayAlertService displayAlertService)
        {
            _productoRepository = productoRepository;
            _displayAlertService = displayAlertService;
            EditarCommand = new Command(async () => await EditarAsync());
            EliminarCommand = new Command(async () => await EliminarAsync());
            VolverCommand = new Command(async () => await Shell.Current.GoToAsync(".."));
        }

        public async Task InicializarAsync()
        {
            try
            {
                IsBusy = true;
                Producto = await _productoRepository.ObtenerPorIdAsync(ProductoId);
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

        public async Task EditarAsync()
        {
            if (Producto == null) return;
            try
            {
                await Shell.Current.GoToAsync($"{nameof(EditarProductoView)}?productoId={Producto.Id}");
            }
            catch (Exception)
            {
                await _displayAlertService.MostrarAlertAsync("Error", "No se pudo navegar a la pantalla de edición", "OK");
            }
        }

        public async Task EliminarAsync()
        {
            if (Producto == null) return;

            try
            {
                var confirmar = await _displayAlertService.MostrarAlertConConfirmacionAsync(
                    "Eliminar", $"¿Eliminar el producto {Producto.Nombre}?", "Eliminar", "Cancelar");

                if (confirmar)
                {
                    await _productoRepository.EliminarAsync(Producto);
                    await Shell.Current.GoToAsync("..");
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
