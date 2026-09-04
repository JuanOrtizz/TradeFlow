using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using TradeFlow.Services;

namespace TradeFlow.ViewModels
{
    public class BackupViewModel : INotifyPropertyChanged
    {
        private static readonly FilePickerFileType TiposBackup = new(
            new Dictionary<DevicePlatform, IEnumerable<string>>
            {
                { DevicePlatform.WinUI, new[] { ".db3" } },
                { DevicePlatform.Android, new[] { "*/*" } },
                { DevicePlatform.iOS, new[] { "public.database", "public.data" } },
                { DevicePlatform.MacCatalyst, new[] { "public.database", "public.data" } }
            });

        private readonly IBackupService _backupService;
        private readonly IDisplayAlertService _displayAlertService;
        private bool _isBusy;

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

        public ICommand CrearBackupCommand { get; }
        public ICommand RestaurarBackupCommand { get; }

        public BackupViewModel(IBackupService backupService, IDisplayAlertService displayAlertService)
        {
            _backupService = backupService;
            _displayAlertService = displayAlertService;
            CrearBackupCommand = new Command(async () => await CrearBackupAsync());
            RestaurarBackupCommand = new Command(async () => await RestaurarBackupAsync());
        }

        public async Task CrearBackupAsync()
        {
            try
            {
                IsBusy = true;

                var nombreArchivo = await _backupService.CrearBackupAsync();

                await _displayAlertService.MostrarAlertAsync("Éxito",
                    $"Backup creado correctamente.\n{nombreArchivo}", "OK");
            }
            catch (Exception)
            {
                await _displayAlertService.MostrarAlertAsync("Error", "No se pudo crear el backup", "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }

        public async Task RestaurarBackupAsync()
        {
            try
            {
                var confirmar = await _displayAlertService.MostrarAlertConConfirmacionAsync(
                    "Restaurar backup",
                    "Se reemplazarán TODOS los datos actuales por los del backup seleccionado. ¿Continuar?",
                    "Restaurar", "Cancelar");

                if (!confirmar) return;

                var archivo = await FilePicker.PickAsync(new PickOptions
                {
                    PickerTitle = "Seleccionar archivo de backup",
                    FileTypes = TiposBackup
                });

                if (archivo == null) return;

                IsBusy = true;

                var nombreRespaldoPrevio = await _backupService.RestaurarBackupAsync(archivo.FullPath);

                await _displayAlertService.MostrarAlertAsync("Éxito",
                    $"Backup restaurado correctamente.\nSe creó un respaldo previo de sus datos: {nombreRespaldoPrevio}", "OK");
            }
            catch (InvalidOperationException ex)
            {
                await _displayAlertService.MostrarAlertAsync("Error", ex.Message, "OK");
            }
            catch (Exception)
            {
                await _displayAlertService.MostrarAlertAsync("Error", "No se pudo restaurar el backup", "OK");
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
