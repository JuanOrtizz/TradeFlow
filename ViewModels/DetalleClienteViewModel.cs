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
    [QueryProperty(nameof(ClienteId), "clienteId")]
    public class DetalleClienteViewModel : INotifyPropertyChanged
    {
        private readonly IClienteRepository _clienteRepository;
        private readonly IFacturaRepository _facturaRepository;
        private readonly ILocalidadRepository _localidadRepository;
        private readonly IDisplayAlertService _displayAlertService;

        private int _clienteId;
        private ClienteModel? _cliente;
        private ObservableCollection<FacturaModel> _listaFacturasCliente = new ObservableCollection<FacturaModel>();
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

        public ObservableCollection<FacturaModel> ListaFacturasCliente
        {
            get => _listaFacturasCliente;
            set
            {
                if (_listaFacturasCliente != value)
                {
                    _listaFacturasCliente = value;
                    OnPropertyChanged(nameof(ListaFacturasCliente));
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

        public ICommand EditarCommand { get; }
        public ICommand EliminarCommand { get; }
        public ICommand VolverCommand { get; }
        public ICommand VerDetalleCommand { get; }

        public DetalleClienteViewModel(IClienteRepository clienteRepository, IFacturaRepository facturaRepository, ILocalidadRepository localidadRepository, IDisplayAlertService displayAlertService)
        {
            _clienteRepository = clienteRepository;
            _facturaRepository = facturaRepository;
            _localidadRepository = localidadRepository;
            _displayAlertService = displayAlertService;
            EditarCommand = new Command(async () => await EditarAsync());
            EliminarCommand = new Command(async () => await EliminarAsync());
            VolverCommand = new Command(async () => await Shell.Current.GoToAsync(".."));
            VerDetalleCommand = new Command<FacturaModel>(async (factura) =>
            {
                if (factura != null)
                {
                    await Shell.Current.GoToAsync($"{nameof(DetalleFacturaView)}?facturaId={factura.Id}");
                }
            });
        }

        public async Task InicializarAsync()
        {
            try
            {
                IsBusy = true;

                var cliente = await _clienteRepository.ObtenerPorIdAsync(ClienteId);
                if (cliente != null)
                {
                    cliente.Localidad = await _localidadRepository.ObtenerPorIdAsync(cliente.LocalidadId);
                }
                Cliente = cliente;

                if (Cliente != null)
                {
                    var facturas = await _facturaRepository.ObtenerPorClienteAsync(ClienteId);
                    ListaFacturasCliente.Clear();
                    foreach (var factura in facturas)
                    {
                        factura.Cliente = Cliente;
                        ListaFacturasCliente.Add(factura);
                    }
                }
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

        public async Task EditarAsync()
        {
            if (Cliente == null) return;
            try
            {
                await Shell.Current.GoToAsync($"{nameof(EditarClienteView)}?clienteId={Cliente.Id}");
            }
            catch (Exception)
            {
                await _displayAlertService.MostrarAlertAsync("Error", "No se pudo navegar a la pantalla de edición", "OK");
            }
        }

        public async Task EliminarAsync()
        {
            if (Cliente == null) return;

            try
            {
                var cantidadFacturas = await _facturaRepository.ContarPorClienteAsync(Cliente.Id);
                if (cantidadFacturas > 0)
                {
                    await _displayAlertService.MostrarAlertAsync(
                        "No se puede eliminar",
                        $"{Cliente.Nombre} tiene {cantidadFacturas} {(cantidadFacturas == 1 ? "factura registrada" : "facturas registradas")}. Eliminá primero sus facturas.",
                        "OK");
                    return;
                }

                var confirmar = await _displayAlertService.MostrarAlertConConfirmacionAsync(
                    "Eliminar", $"¿Eliminar el cliente {Cliente.Nombre}?", "Eliminar", "Cancelar");

                if (confirmar)
                {
                    await _clienteRepository.EliminarAsync(Cliente);
                    await Shell.Current.GoToAsync("..");
                }
            }
            catch (Exception)
            {
                await _displayAlertService.MostrarAlertAsync("Error", "No se pudo eliminar el cliente", "OK");
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
