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
    public class FacturasViewModel : INotifyPropertyChanged
    {
        private readonly IFacturaRepository _facturaRepository;
        private readonly IClienteRepository _clienteRepository;
        private readonly IDisplayAlertService _displayAlertService;

        private ObservableCollection<FacturaModel> _listaFacturas = new ObservableCollection<FacturaModel>();
        private bool _isBusy;
        private string _textoBusqueda = string.Empty;
        private bool _filtraPorFecha;
        private DateTime _fechaSeleccionada = DateTime.Today;
        private int _idBusquedaActual;

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

        public bool FiltraPorFecha
        {
            get => _filtraPorFecha;
            set
            {
                if (_filtraPorFecha != value)
                {
                    _filtraPorFecha = value;
                    OnPropertyChanged(nameof(FiltraPorFecha));
                    OnPropertyChanged(nameof(FiltroHoyActivo));
                    OnPropertyChanged(nameof(FiltroTodasActivo));
                }
            }
        }

        public DateTime FechaSeleccionada
        {
            get => _fechaSeleccionada;
            set
            {
                if (_fechaSeleccionada != value)
                {
                    _fechaSeleccionada = value;
                    OnPropertyChanged(nameof(FechaSeleccionada));
                    OnPropertyChanged(nameof(FiltroHoyActivo));
                    OnPropertyChanged(nameof(FiltroTodasActivo));
                }
            }
        }

        // Indican que opcion del filtro esta seleccionada para marcar el subrayado
        public bool FiltroHoyActivo => FiltraPorFecha && FechaSeleccionada.Date == DateTime.Today;
        public bool FiltroTodasActivo => !FiltraPorFecha;

        public ICommand VerDetalleCommand { get; }
        public ICommand IrAAgregarCommand { get; }
        public ICommand BuscarCommand { get; }

        public FacturasViewModel(IFacturaRepository facturaRepository, IClienteRepository clienteRepository, IDisplayAlertService displayAlertService)
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
            IrAAgregarCommand = new Command(async () => await Shell.Current.GoToAsync(nameof(CrearFacturaView)));
            BuscarCommand = new Command(async () => await BuscarAsync());
        }

        public async Task InicializarAsync()
        {
            TextoBusqueda = string.Empty;
            FiltraPorFecha = false;
            FechaSeleccionada = DateTime.Today;

            await BuscarAsync();
        }

        public async Task BuscarAsync()
        {
            var idBusqueda = ++_idBusquedaActual;

            try
            {
                IsBusy = true;

                IReadOnlyList<FacturaModel> facturas;

                if (FiltraPorFecha)
                {
                    facturas = await _facturaRepository.ObtenerPorFechaAsync(FechaSeleccionada.Date);

                    var terminoConFecha = TextoBusqueda?.Trim() ?? string.Empty;
                    if (!string.IsNullOrEmpty(terminoConFecha))
                    {
                        facturas = facturas.Where(f => f.Id.ToString().Contains(terminoConFecha)).ToList();
                    }
                }
                else
                {
                    var termino = TextoBusqueda?.Trim() ?? string.Empty;
                    facturas = string.IsNullOrEmpty(termino)
                        ? await _facturaRepository.ObtenerTodasAsync()
                        : await _facturaRepository.BuscarPorNumeroAsync(termino);
                }

                foreach (var factura in facturas)
                {
                    factura.Cliente = factura.ClienteId > 0
                        ? await _clienteRepository.ObtenerPorIdAsync(factura.ClienteId)
                        : null;
                }

                if (idBusqueda != _idBusquedaActual) return;

                ListaFacturas.Clear();
                foreach (var factura in facturas)
                {
                    ListaFacturas.Add(factura);
                }
            }
            catch (Exception)
            {
                if (idBusqueda != _idBusquedaActual) return;
                await _displayAlertService.MostrarAlertAsync("Error", "No se pudieron cargar las facturas", "OK");
            }
            finally
            {
                if (idBusqueda == _idBusquedaActual)
                {
                    IsBusy = false;
                }
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
