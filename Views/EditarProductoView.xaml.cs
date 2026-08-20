using TradeFlow.ViewModels;

namespace TradeFlow.Views;

public partial class EditarProductoView : ContentPage
{
    public EditarProductoView(EditarProductoViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        if (BindingContext is EditarProductoViewModel vm)
        {
            await vm.InicializarAsync();
        }
    }
}
