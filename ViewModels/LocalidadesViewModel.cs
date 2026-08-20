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
    public class LocalidadesViewModel
    {
        // Servicios y repositorios
        private readonly ILocalidadRepository _localidadRepository;
        private readonly IDisplayAlertService _displayAlertService;

        // Propiedades privadas
        private ObservableCollection<LocalidadModel> _listaLocalidades = new ObservableCollection<LocalidadModel>();
        private bool _isBusy;

        // Propiedades públicas
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

        public ICommand EliminarCommand { get; }
        public ICommand IrAAgregarCommand { get; }

        public LocalidadesViewModel(ILocalidadRepository localidadRepository, IDisplayAlertService displayAlertService)
        {
            _localidadRepository = localidadRepository;
            _displayAlertService = displayAlertService;
            EliminarCommand = new Command<LocalidadModel>(async (localidad) => await EliminarAsync(localidad));
            IrAAgregarCommand = new Command(async () => await Shell.Current.GoToAsync(nameof(AgregarLocalidadView)));
        }

        public async Task InicializarAsync()
        {
            try
            {
                IsBusy = true;
                _listaLocalidades.Clear();

                var localidades = await _localidadRepository.ObtenerTodasAsync();
                foreach (var localidad in localidades)
                {
                    _listaLocalidades.Add(localidad);
                }
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

        public async Task EliminarAsync(LocalidadModel localidad)
        {
            try
            {
                var confirmar = await _displayAlertService.MostrarAlertConConfirmacionAsync(
                    "Eliminar", $"¿Eliminar la localidad {localidad.Nombre}?", "Eliminar", "Cancelar");

                if (confirmar)
                {
                    await _localidadRepository.EliminarAsync(localidad);
                    _listaLocalidades.Remove(localidad);
                }
            }
            catch (Exception)
            {
                await _displayAlertService.MostrarAlertAsync("Error", "No se pudo eliminar la localidad", "OK");
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
