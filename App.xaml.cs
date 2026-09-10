using TradeFlow.Data;

namespace TradeFlow;

public partial class App : Application
{
    private readonly DatabaseService _dbService;

    public App(DatabaseService dbService)
    {
        InitializeComponent();
        _dbService = dbService;

        // Fuerzo modo claro siempre, sin importar el tema del sistema
        UserAppTheme = AppTheme.Light;
    }

    protected override Window CreateWindow(IActivationState? activationState)
    {
        return new Window(new AppShell());
    }

    protected override async void OnStart()
    {
        base.OnStart();

        // Corre en segundo plano sin congelar el hilo STA de arranque de Windows
        await _dbService.InitializeAsync();
    }
}