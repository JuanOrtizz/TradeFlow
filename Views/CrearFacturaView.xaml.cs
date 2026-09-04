using TradeFlow.ViewModels;

namespace TradeFlow.Views;

public partial class CrearFacturaView : ContentPage
{
    public CrearFacturaView(CrearFacturaViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        if (BindingContext is CrearFacturaViewModel vm)
        {
            await vm.InicializarAsync();
        }
    }
}
