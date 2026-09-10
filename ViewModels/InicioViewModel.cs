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
    public class InicioViewModel : INotifyPropertyChanged
    {
        private readonly IFacturaRepository _facturaRepository;
        private readonly IClienteRepository _clienteRepository;
        private readonly IDisplayAlertService _displayAlertService;

        private ObservableCollection<FacturaModel> _listaFacturas = new ObservableCollection<FacturaModel>();
        private bool _isBusy;

        public ObservableCollection<FacturaModel> ListaFacturas
        {
            get => _listaFacturas;
            set
            {
                if (_listaFacturas != value)
                {
                    _listaFacturas = value;
                    OnPropertyChanged(nameof(ListaFacturas));
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

        public ICommand VerDetalleCommand { get; }

        public InicioViewModel(IFacturaRepository facturaRepository, IClienteRepository clienteRepository, IDisplayAlertService displayAlertService)
        {
            _facturaRepository = facturaRepository;
            _clienteRepository = clienteRepository;
            _displayAlertService = displayAlertService;

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

                var facturas = await _facturaRepository.ObtenerUltimasDiezAsync();
                var clientes = await _clienteRepository.ObtenerPorIdsAsync(facturas.Select(f => f.ClienteId));
                var clientesDict = clientes.ToDictionary(c => c.Id);

                foreach (var factura in facturas)
                {
                    clientesDict.TryGetValue(factura.ClienteId, out var cliente);
                    factura.Cliente = cliente;
                }
                _listaFacturas = new ObservableCollection<FacturaModel>(facturas);
                OnPropertyChanged(nameof(ListaFacturas));
            }
            catch (Exception)
            {
                await _displayAlertService.MostrarAlertAsync("Error", "No se pudo inicializar el menu principal", "OK");
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
