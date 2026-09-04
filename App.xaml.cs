namespace TradeFlow
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            // Fuerzo modo claro siempre, sin importar el tema del sistema
            UserAppTheme = AppTheme.Light;

            MainPage = new AppShell();
        }
    }
}
