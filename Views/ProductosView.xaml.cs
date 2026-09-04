using TradeFlow.Models;
using TradeFlow.ViewModels;

namespace TradeFlow.Views;

public partial class ProductosView : ContentPage
{
    public ProductosView(ProductosViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        if (BindingContext is ProductosViewModel vm)
        {
            await vm.InicializarAsync();
        }
    }

    private async void OnEstadoChanged(object? sender, CheckedChangedEventArgs e)
    {
        if (sender is CheckBox checkBox &&
            BindingContext is ProductosViewModel vm &&
            checkBox.BindingContext is ProductoModel producto)
        {
            await vm.CambiarEstadoAsync(producto);
        }
    }
}
