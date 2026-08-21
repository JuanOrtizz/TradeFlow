using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using TradeFlow.Data.Repositories;
using TradeFlow.Models;
using TradeFlow.Services;

namespace TradeFlow.ViewModels
{
    [QueryProperty(nameof(ClienteId), "clienteId")]
    public class DetalleClienteViewModel
    {
        private readonly IClienteRepository _clienteRepository;
        private readonly IDisplayAlertService _displayAlertService;

        private int _clienteId;
        private ClienteModel? _cliente;
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

        public DetalleClienteViewModel(IClienteRepository clienteRepository, IDisplayAlertService displayAlertService)
        {
            _clienteRepository = clienteRepository;
            _displayAlertService = displayAlertService;
            EditarCommand = new Command(async () => await EditarAsync());
            EliminarCommand = new Command(async () => await EliminarAsync());
        }

        public async Task InicializarAsync()
        {
            try
            {
                IsBusy = true;
                Cliente = await _clienteRepository.ObtenerPorIdAsync(ClienteId);
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
                await Shell.Current.GoToAsync($"editarcliente?clienteId={Cliente.Id}");
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
