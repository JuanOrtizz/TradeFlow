using TradeFlow.ViewModels;

namespace TradeFlow.Views;

public partial class VistaPreviaFacturaView : ContentPage
{
    public VistaPreviaFacturaView(VistaPreviaFacturaViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        if (BindingContext is VistaPreviaFacturaViewModel vm)
        {
            await vm.InicializarAsync();
        }
    }
}
