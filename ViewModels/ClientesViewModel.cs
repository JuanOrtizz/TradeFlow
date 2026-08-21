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
    public class ClientesViewModel
    {
        private readonly IClienteRepository _clienteRepository;
        private readonly IDisplayAlertService _displayAlertService;

        private ObservableCollection<ClienteModel> _listaClientes = new ObservableCollection<ClienteModel>();
        private bool _isBusy;
        private string _textoBusqueda = string.Empty;

        public ObservableCollection<ClienteModel> ListaClientes
        {
            get => _listaClientes;
            set
            {
                if (_listaClientes != value)
                {
                    _listaClientes = value;
                    OnPropertyChanged(nameof(ListaClientes));
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

        public ClientesViewModel(IClienteRepository clienteRepository, IDisplayAlertService displayAlertService)
        {
            _clienteRepository = clienteRepository;
            _displayAlertService = displayAlertService;
            EliminarCommand = new Command<ClienteModel>(async (cliente) => await EliminarAsync(cliente));
            VerDetalleCommand = new Command<ClienteModel>(async (cliente) => await Shell.Current.GoToAsync($"detallecliente?clienteId={cliente.Id}"));
            IrAAgregarCommand = new Command(async () => await Shell.Current.GoToAsync(nameof(AgregarClienteView)));
            BuscarCommand = new Command(async () => await BuscarAsync());
        }

        public async Task InicializarAsync()
        {
            try
            {
                IsBusy = true;
                _listaClientes.Clear();

                var clientes = await _clienteRepository.ObtenerTodosAsync();
                foreach (var cliente in clientes)
                {
                    _listaClientes.Add(cliente);
                }
            }
            catch (Exception)
            {
                await _displayAlertService.MostrarAlertAsync("Error", "No se pudieron cargar los clientes", "OK");
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
                _listaClientes.Clear();

                var clientes = string.IsNullOrWhiteSpace(TextoBusqueda)
                    ? await _clienteRepository.ObtenerTodosAsync()
                    : await _clienteRepository.BuscarPorNombreAsync(TextoBusqueda.Trim());

                foreach (var cliente in clientes)
                {
                    _listaClientes.Add(cliente);
                }
            }
            catch (Exception)
            {
                await _displayAlertService.MostrarAlertAsync("Error", "No se pudieron buscar los clientes", "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }

        public async Task EliminarAsync(ClienteModel cliente)
        {
            try
            {
                var confirmar = await _displayAlertService.MostrarAlertConConfirmacionAsync(
                    "Eliminar", $"¿Eliminar el cliente {cliente.Nombre}?", "Eliminar", "Cancelar");

                if (confirmar)
                {
                    await _clienteRepository.EliminarAsync(cliente);
                    _listaClientes.Remove(cliente);
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
