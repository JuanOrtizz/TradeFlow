using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using TradeFlow.Data.Repositories;
using TradeFlow.Models;
using TradeFlow.Services;

namespace TradeFlow.ViewModels
{
    [QueryProperty(nameof(FacturaId), "facturaId")]
    public class VistaPreviaFacturaViewModel : INotifyPropertyChanged
    {
        private readonly IFacturaRepository _facturaRepository;
        private readonly IClienteRepository _clienteRepository;
        private readonly ILocalidadRepository _localidadRepository;
        private readonly IImpresionService _impresionService;
        private readonly IDisplayAlertService _displayAlertService;

        private int _facturaId;
        private bool _isBusy;
        private string _htmlFactura = string.Empty;

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

        public string HtmlFactura
        {
            get => _htmlFactura;
            set
            {
                if (_htmlFactura != value)
                {
                    _htmlFactura = value;
                    OnPropertyChanged();
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
                    OnPropertyChanged();
                }
            }
        }

        public ICommand VolverCommand { get; }
        public ICommand ImprimirCommand { get; }

        public VistaPreviaFacturaViewModel(
            IFacturaRepository facturaRepository,
            IClienteRepository clienteRepository,
            ILocalidadRepository localidadRepository,
            IImpresionService impresionService,
            IDisplayAlertService displayAlertService)
        {
            _facturaRepository = facturaRepository;
            _clienteRepository = clienteRepository;
            _localidadRepository = localidadRepository;
            _impresionService = impresionService;
            _displayAlertService = displayAlertService;

            VolverCommand = new Command(async () => await Shell.Current.GoToAsync(".."));
            ImprimirCommand = new Command<object>(async (controlWebView) => await ImprimirAsync(controlWebView));
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
                if (factura.Cliente != null)
                {
                    factura.Cliente.Localidad = await _localidadRepository.ObtenerPorIdAsync(factura.Cliente.LocalidadId);
                }

                var items = await _facturaRepository.ObtenerDetallesAsync(FacturaId);
                HtmlFactura = _impresionService.GenerarHtmlFactura(factura, items);
            }
            catch (Exception)
            {
                await _displayAlertService.MostrarAlertAsync("Error", "No se pudo generar la vista previa", "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async Task ImprimirAsync(object? controlWebView)
        {
            if (string.IsNullOrEmpty(HtmlFactura)) return;

            try
            {
                await _impresionService.ImprimirAsync(controlWebView!);
            }
            catch (Exception)
            {
                await _displayAlertService.MostrarAlertAsync("Error", "No se pudo abrir el dialogo de impresion", "OK");
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
