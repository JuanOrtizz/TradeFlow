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
    [QueryProperty(nameof(FacturaId), "facturaId")]
    public class DetalleFacturaViewModel : INotifyPropertyChanged
    {
        private readonly IFacturaRepository _facturaRepository;
        private readonly IClienteRepository _clienteRepository;
        private readonly IDisplayAlertService _displayAlertService;

        private int _facturaId;
        private FacturaModel? _factura;
        private ObservableCollection<DetalleFacturaModel> _items = new ObservableCollection<DetalleFacturaModel>();
        private bool _isBusy;

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

        public ObservableCollection<DetalleFacturaModel> Items
        {
            get => _items;
            set
            {
                if (_items != value)
                {
                    _items = value;
                    OnPropertyChanged(nameof(Items));
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

        // Comandos
        public ICommand VolverCommand { get; }
        public ICommand EliminarCommand { get; }
        public ICommand IrAImprimirCommand { get; }

        public DetalleFacturaViewModel(IFacturaRepository facturaRepository, IClienteRepository clienteRepository, IDisplayAlertService displayAlertService)
        {
            _facturaRepository = facturaRepository;
            _clienteRepository = clienteRepository;
            _displayAlertService = displayAlertService;
            VolverCommand = new Command(async () => await Shell.Current.GoToAsync(".."));
            EliminarCommand = new Command(async () => await EliminarAsync());
            IrAImprimirCommand = new Command(async () =>
            {
                if (Factura != null)
                {
                    await Shell.Current.GoToAsync($"{nameof(VistaPreviaFacturaView)}?facturaId={Factura.Id}");
                }
            });
        }

        public async Task EliminarAsync()
        {
            if (Factura == null) return;

            try
            {
                var confirmar = await _displayAlertService.MostrarAlertConConfirmacionAsync(
                    "Eliminar", $"¿Eliminar la factura #{Factura.Id}?", "Eliminar", "Cancelar");

                if (confirmar)
                {
                    await _facturaRepository.EliminarAsync(Factura);
                    await Shell.Current.GoToAsync("..");
                }
            }
            catch (Exception)
            {
                await _displayAlertService.MostrarAlertAsync("Error", "No se pudo eliminar la factura", "OK");
            }
        }

        public async Task InicializarAsync()
        {
            try
            {
                IsBusy = true;

                var factura = await _facturaRepository.ObtenerPorIdAsync(FacturaId);
                if (factura != null)
                {
                    // Hidrato el cliente ANTES de asignar la factura para que los bindings
                    // evalúen con el dato completo
                    factura.Cliente = await _clienteRepository.ObtenerPorIdAsync(factura.ClienteId);

                    var detalles = await _facturaRepository.ObtenerDetallesAsync(FacturaId);
                    Items.Clear();
                    foreach (var item in detalles)
                    {
                        Items.Add(item);
                    }

                    Factura = factura;
                }
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

        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
