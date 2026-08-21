using TradeFlow.ViewModels;

namespace TradeFlow.Views;

public partial class DetalleClienteView : ContentPage
{
    public DetalleClienteView(DetalleClienteViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        if (BindingContext is DetalleClienteViewModel vm)
        {
            await vm.InicializarAsync();
        }
    }
}
