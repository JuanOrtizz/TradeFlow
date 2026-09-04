using TradeFlow.ViewModels;

namespace TradeFlow.Views;

public partial class ClientesView : ContentPage
{
    public ClientesView(ClientesViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        if (BindingContext is ClientesViewModel vm)
        {
            await vm.InicializarAsync();
        }
    }
}
