using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using TradeFlow.Data.Repositories;
using TradeFlow.Models;
using TradeFlow.Services;

namespace TradeFlow.ViewModels
{
    public class InicioViewModel
    {
        //Servicios y repositorios
        private readonly IFacturaRepository _facturaRepository;
        private readonly IDisplayAlertService _displayAlertService;

        // Propiedades privadas
        private ObservableCollection<FacturaModel> _listaFacturas = new ObservableCollection<FacturaModel>();
        private bool _isBusy;

        // Propiedades públicas
        public ObservableCollection<FacturaModel> ListaFacturas
        {
            get => _listaFacturas;
            set
            {
                if(_listaFacturas != value)
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
                if(_isBusy != value)
                {
                    _isBusy = value;
                    OnPropertyChanged(nameof(IsBusy));
                }
            }
        }

        // Constructor
        public InicioViewModel(IFacturaRepository facturaRepository, IDisplayAlertService displayAlertService)
        {
            _facturaRepository = facturaRepository;
            _displayAlertService = displayAlertService;
        }

        public async Task InicializarAsync()
        {
            try
            {
                // Muestro spinner de carga
                IsBusy = true;
                _listaFacturas.Clear();
                // Obtengo todas las facturas
                var facturas = await _facturaRepository.ObtenerUltimasDiezAsync();
                foreach (var factura in facturas)
                {
                    _listaFacturas.Add(factura);
                }
            }
            catch (Exception)
            {
                await _displayAlertService.MostrarAlertAsync("Error", "No se pudo inicializar el menú principal", "OK");
            }
            finally
            {
                // Oculto spinner de carga
                IsBusy = false;
            }
        }

        // Implementacion de INotifyPropertyChanged
        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
