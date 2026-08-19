using TradeFlow.ViewModels;

namespace TradeFlow.Views;

public partial class InicioView : ContentPage
{
    private bool _inicializado = false;

    public InicioView(InicioViewModel vm)
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
            if (BindingContext is InicioViewModel vm)
            {
                await vm.InicializarAsync();
            }
        }
    }
}
