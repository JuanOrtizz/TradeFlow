using Microsoft.Extensions.Logging;
using TradeFlow.Data;
using TradeFlow.Data.Repositories;
using TradeFlow.ViewModels;
using TradeFlow.Services;
using TradeFlow.Views;
using CommunityToolkit.Maui;

namespace TradeFlow
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .UseMauiCommunityToolkit()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

#if WINDOWS
            builder.ConfigureMauiHandlers(handlers =>
            {
                handlers.AddHandler<Microsoft.Maui.Controls.Button, TradeFlow.Platforms.Windows.HandCursorButtonHandler>();
            });

            Microsoft.Maui.Handlers.WindowHandler.Mapper.AppendToMapping("CustomTitleBar", (handler, view) =>
            {
                handler.PlatformView.ExtendsContentIntoTitleBar = false;
            });
#endif

            // Base de datos
            builder.Services.AddSingleton<DatabaseService>(sp =>
            {
                Directory.CreateDirectory(TradeFlow.Helpers.AppPaths.DirectorioDatos);
                return new DatabaseService(TradeFlow.Helpers.AppPaths.ObtenerRuta("tradeflow.db3"));
            });

            // Servicios
            builder.Services.AddSingleton<IDisplayAlertService, DisplayAlertService>();
            builder.Services.AddSingleton<IValidacionesService, ValidacionesService>();
            builder.Services.AddSingleton<IBackupService, BackupService>();
            builder.Services.AddSingleton<IImpresionService, ImpresionService>();

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
            builder.Services.AddTransient<VistaPreviaFacturaViewModel>();
            builder.Services.AddTransient<DetalleProductoViewModel>();
            builder.Services.AddTransient<AgregarProductoViewModel>();
            builder.Services.AddTransient<DetalleClienteViewModel>();
            builder.Services.AddTransient<AgregarClienteViewModel>();
            builder.Services.AddTransient<AgregarLocalidadViewModel>();
            builder.Services.AddTransient<EditarProductoViewModel>();
            builder.Services.AddTransient<EditarClienteViewModel>();

            // Views
            builder.Services.AddTransient<InicioView>();
            builder.Services.AddTransient<FacturasView>();
            builder.Services.AddTransient<ProductosView>();
            builder.Services.AddTransient<ClientesView>();
            builder.Services.AddTransient<LocalidadesView>();
            builder.Services.AddTransient<BackupView>();
            builder.Services.AddTransient<CrearFacturaView>();
            builder.Services.AddTransient<DetalleFacturaView>();
            builder.Services.AddTransient<VistaPreviaFacturaView>();
            builder.Services.AddTransient<DetalleProductoView>();
            builder.Services.AddTransient<AgregarProductoView>();
            builder.Services.AddTransient<DetalleClienteView>();
            builder.Services.AddTransient<AgregarClienteView>();
            builder.Services.AddTransient<AgregarLocalidadView>();

#if DEBUG
            builder.Logging.AddDebug();
#endif

            // Construyo la app y la retorno directamente sin bloquear el hilo
            var app = builder.Build();

            return app;
        }
    }
}
