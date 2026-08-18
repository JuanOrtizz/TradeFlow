using Microsoft.Extensions.Logging;
using TradeFlow.Data;
using TradeFlow.Data.Repositories;
using TradeFlow.ViewModels;
using TradeFlow.Views;

namespace TradeFlow
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

            // Base de datos
            builder.Services.AddSingleton<DatabaseService>(sp =>
            {
                var dbPath = Path.Combine(FileSystem.AppDataDirectory, "tradeflow.db3");
                return new DatabaseService(dbPath);
            });

            // Repositorios
            builder.Services.AddSingleton<IClienteRepository, ClienteRepository>();
            builder.Services.AddSingleton<IProductoRepository, ProductoRepository>();
            builder.Services.AddSingleton<IFacturaRepository, FacturaRepository>();
            builder.Services.AddSingleton<ILocalidadRepository, LocalidadRepository>();

            // ViewModels
            builder.Services.AddTransient<InicioViewModel>();
            builder.Services.AddTransient<FacturasViewModel>();
            builder.Services.AddTransient<ProductosViewModel>();
            builder.Services.AddTransient<ClientesViewModel>();
            builder.Services.AddTransient<LocalidadesViewModel>();
            builder.Services.AddTransient<BackupViewModel>();
            builder.Services.AddTransient<CrearFacturaViewModel>();
            builder.Services.AddTransient<DetalleFacturaViewModel>();
            builder.Services.AddTransient<DetalleProductoViewModel>();
            builder.Services.AddTransient<AgregarProductoViewModel>();
            builder.Services.AddTransient<DetalleClienteViewModel>();
            builder.Services.AddTransient<AgregarClienteViewModel>();
            builder.Services.AddTransient<AgregarLocalidadViewModel>();

            // Views
            builder.Services.AddTransient<InicioView>();
            builder.Services.AddTransient<FacturasView>();
            builder.Services.AddTransient<ProductosView>();
            builder.Services.AddTransient<ClientesView>();
            builder.Services.AddTransient<LocalidadesView>();
            builder.Services.AddTransient<BackupView>();
            builder.Services.AddTransient<CrearFacturaView>();
            builder.Services.AddTransient<DetalleFacturaView>();
            builder.Services.AddTransient<DetalleProductoView>();
            builder.Services.AddTransient<AgregarProductoView>();
            builder.Services.AddTransient<DetalleClienteView>();
            builder.Services.AddTransient<AgregarClienteView>();
            builder.Services.AddTransient<AgregarLocalidadView>();

#if DEBUG
            builder.Logging.AddDebug();
#endif
            // Construyo la app e inicio la DB
            var app = builder.Build();
            _ = InicializarAsync(app);

            return app;
        }

        private static async Task InicializarAsync(MauiApp app)
        {
            try
            {
                using var scope = app.Services.CreateScope();
                var db = scope.ServiceProvider.GetRequiredService<DatabaseService>();
                await db.InitializeAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error inicializando la BD: {ex.Message}");
            }
        }
    }
}
