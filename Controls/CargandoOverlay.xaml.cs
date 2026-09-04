using Microsoft.Maui.Controls;

namespace TradeFlow.Controls
{
    // Overlay de pantalla completa que bloquea los toques mientras EstaCargando es true.
    // Uso en XAML: <controls:CargandoOverlay EstaCargando="{Binding IsBusy}" />
    public partial class CargandoOverlay : ContentView
    {
        public static readonly BindableProperty EstaCargandoProperty =
            BindableProperty.Create(nameof(EstaCargando), typeof(bool), typeof(CargandoOverlay), false);

        public bool EstaCargando
        {
            get => (bool)GetValue(EstaCargandoProperty);
            set => SetValue(EstaCargandoProperty, value);
        }

        public CargandoOverlay()
        {
            InitializeComponent();
        }
    }
}
