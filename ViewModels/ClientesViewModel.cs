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
    public class ClientesViewModel : INotifyPropertyChanged
    {
        private readonly IClienteRepository _clienteRepository;
        private readonly ILocalidadRepository _localidadRepository;
        private readonly IDisplayAlertService _displayAlertService;

        private ObservableCollection<ClienteModel> _listaClientes = new ObservableCollection<ClienteModel>();
        private ObservableCollection<LocalidadModel> _listaLocalidades = new ObservableCollection<LocalidadModel>();
        private bool _isBusy;
        private string _textoBusqueda = string.Empty;
        private int _indiceLocalidad;
        private int _localidadIdSeleccionada;
        private int _idBusquedaActual;

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

        public ObservableCollection<LocalidadModel> ListaLocalidades
        {
            get => _listaLocalidades;
            set
            {
                if (_listaLocalidades != value)
                {
                    _listaLocalidades = value;
                    OnPropertyChanged(nameof(ListaLocalidades));
                }
            }
        }

        public int LocalidadIdSeleccionada
        {
            get => _localidadIdSeleccionada;
            set
            {
                if (_localidadIdSeleccionada != value)
                {
                    _localidadIdSeleccionada = value;
                    OnPropertyChanged(nameof(LocalidadIdSeleccionada));
                }
            }
        }

        public int IndiceLocalidad
        {
            get => _indiceLocalidad;
            set
            {
                if (_indiceLocalidad != value)
                {
                    _indiceLocalidad = value;
                    OnPropertyChanged(nameof(IndiceLocalidad));
                    LocalidadIdSeleccionada = value > 0 && value < ListaLocalidades.Count
                        ? ListaLocalidades[value].Id
                        : 0;
                    _ = BuscarAsync();
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
                    _ = BuscarAsync();
                }
            }
        }

        public ICommand VerDetalleCommand { get; }
        public ICommand IrAAgregarCommand { get; }

        public ClientesViewModel(IClienteRepository clienteRepository, ILocalidadRepository localidadRepository, IDisplayAlertService displayAlertService)
        {
            _clienteRepository = clienteRepository;
            _localidadRepository = localidadRepository;
            _displayAlertService = displayAlertService;
            VerDetalleCommand = new Command<ClienteModel>(async (cliente) => await Shell.Current.GoToAsync($"{nameof(DetalleClienteView)}?clienteId={cliente.Id}"));
            IrAAgregarCommand = new Command(async () => await Shell.Current.GoToAsync(nameof(AgregarClienteView)));
        }

        public async Task InicializarAsync()
        {
            try
            {
                IsBusy = true;

                var localidades = await _localidadRepository.ObtenerTodasAsync();

                _listaLocalidades.Clear();
                _listaLocalidades.Add(new LocalidadModel { Id = 0, Nombre = "Todas" });
                foreach (var localidad in localidades)
                {
                    _listaLocalidades.Add(localidad);
                }

                _indiceLocalidad = -1;
                IndiceLocalidad = 0;
                await BuscarAsync();
            }
            catch (Exception)
            {
                await _displayAlertService.MostrarAlertAsync("Error", "No se pudieron cargar las localidades", "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }

        public async Task BuscarAsync()
        {
            var idBusqueda = ++_idBusquedaActual;

            try
            {
                IsBusy = true;

                var clientes = string.IsNullOrWhiteSpace(TextoBusqueda)
                    ? await _clienteRepository.ObtenerTodosAsync()
                    : await _clienteRepository.BuscarPorNombreAsync(TextoBusqueda.Trim());

                if (LocalidadIdSeleccionada > 0)
                {
                    clientes = clientes.Where(c => c.LocalidadId == LocalidadIdSeleccionada).ToList();
                }

                if (idBusqueda != _idBusquedaActual) return;

                _listaClientes.Clear();
                foreach (var cliente in clientes)
                {
                    _listaClientes.Add(cliente);
                }
            }
            catch (Exception)
            {
                if (idBusqueda != _idBusquedaActual) return;
                await _displayAlertService.MostrarAlertAsync("Error", "No se pudieron buscar los clientes", "OK");
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
