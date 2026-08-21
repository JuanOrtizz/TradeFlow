using TradeFlow.ViewModels;

namespace TradeFlow.Views;

public partial class ClientesView : ContentPage
{
    private bool _inicializado = false;

    public ClientesView(ClientesViewModel vm)
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
            if (BindingContext is ClientesViewModel vm)
            {
                await vm.InicializarAsync();
            }
        }
    }
}
