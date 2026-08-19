using TradeFlow.ViewModels;

namespace TradeFlow.Views;

public partial class LocalidadesView : ContentPage
{
    private bool _inicializado = false;

    public LocalidadesView(LocalidadesViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        if (!_inicializado)
        {
            _inicializado = true;
            if (BindingContext is LocalidadesViewModel vm)
            {
                await vm.InicializarAsync();
            }
        }
    }
}
