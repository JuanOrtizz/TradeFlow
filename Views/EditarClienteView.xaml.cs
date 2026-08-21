using TradeFlow.ViewModels;

namespace TradeFlow.Views;

public partial class EditarClienteView : ContentPage
{
    public EditarClienteView(EditarClienteViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        if (BindingContext is EditarClienteViewModel vm)
        {
            await vm.InicializarAsync();
        }
    }
}
