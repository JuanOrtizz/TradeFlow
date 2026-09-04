namespace TradeFlow.Services
{
    public interface IBackupService
    {
        Task<string> CrearBackupAsync();
        Task<string> RestaurarBackupAsync(string archivoOrigen);
    }
}
