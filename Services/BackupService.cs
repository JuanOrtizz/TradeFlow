using TradeFlow.Data;

namespace TradeFlow.Services
{
    public class BackupService : IBackupService
    {
        private readonly DatabaseService _databaseService;

        public BackupService(DatabaseService databaseService)
        {
            _databaseService = databaseService;
        }

        public async Task<string> CrearBackupAsync()
        {
            if (!File.Exists(_databaseService.DbPath))
            {
                throw new FileNotFoundException("No se encontró la base de datos.");
            }

            var backupFileName = GenerarNombreBackup();
            var carpetaBackups = ObtenerCarpetaBackups();

            File.Copy(_databaseService.DbPath, Path.Combine(carpetaBackups, backupFileName), true);

            await Task.CompletedTask;
            return backupFileName;
        }

        public async Task<string> RestaurarBackupAsync(string archivoOrigen)
        {
            if (!File.Exists(archivoOrigen))
            {
                throw new FileNotFoundException("No se encontró el archivo de backup seleccionado.");
            }

            var nombreRespaldoPrevio = await CrearBackupAsync();
            var rutaRespaldoPrevio = Path.Combine(ObtenerCarpetaBackups(), nombreRespaldoPrevio);

            try
            {
                await _databaseService.CerrarConexionAsync();
                File.Copy(archivoOrigen, _databaseService.DbPath, true);
                await _databaseService.InitializeAsync();
            }
            catch (Exception)
            {
                await RevertirRestauracionAsync(rutaRespaldoPrevio);
                throw new InvalidOperationException(
                    "El archivo seleccionado no es una base de datos válida. Se conservó la base actual.");
            }

            return nombreRespaldoPrevio;
        }

        private async Task RevertirRestauracionAsync(string rutaRespaldoPrevio)
        {
            try
            {
                await _databaseService.CerrarConexionAsync();
                File.Copy(rutaRespaldoPrevio, _databaseService.DbPath, true);
                await _databaseService.InitializeAsync();
            }
            catch (Exception)
            {
                throw new InvalidOperationException(
                    "No se pudo restaurar el backup y falló la recuperación automática. " +
                    $"Copie manualmente el respaldo '{Path.GetFileName(rutaRespaldoPrevio)}' desde la carpeta Backups.");
            }
        }

        private static string ObtenerCarpetaBackups()
        {
            var carpetaBackups = Path.Combine(FileSystem.AppDataDirectory, "Backups");
            Directory.CreateDirectory(carpetaBackups);
            return carpetaBackups;
        }

        private static string GenerarNombreBackup()
        {
            return $"tradeflow_backup_{DateTime.Now:yyyyMMdd_HHmmss}.db3";
        }
    }
}
