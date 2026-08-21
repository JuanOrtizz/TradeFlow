using TradeFlow.Views;

namespace TradeFlow
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();

            // Rutas secundarias (navegación programática)
            Routing.RegisterRoute(nameof(CrearFacturaView), typeof(CrearFacturaView));
            Routing.RegisterRoute(nameof(DetalleFacturaView), typeof(DetalleFacturaView));
            Routing.RegisterRoute(nameof(AgregarProductoView), typeof(AgregarProductoView));
            Routing.RegisterRoute(nameof(DetalleProductoView), typeof(DetalleProductoView));
            Routing.RegisterRoute(nameof(AgregarClienteView), typeof(AgregarClienteView));
            Routing.RegisterRoute(nameof(DetalleClienteView), typeof(DetalleClienteView));
            Routing.RegisterRoute(nameof(AgregarLocalidadView), typeof(AgregarLocalidadView));
            Routing.RegisterRoute(nameof(EditarProductoView), typeof(EditarProductoView));
            Routing.RegisterRoute(nameof(EditarClienteView), typeof(EditarClienteView));
        }
    }
}
