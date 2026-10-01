using TradeFlow.ViewModels;

namespace TradeFlow.Views;

public partial class EditarFacturaView : ContentPage
{
    public EditarFacturaView(EditarFacturaViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        if (BindingContext is EditarFacturaViewModel vm)
        {
            await vm.InicializarAsync();
        }
    }
}
