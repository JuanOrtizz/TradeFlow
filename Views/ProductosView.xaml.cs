using TradeFlow.ViewModels;

namespace TradeFlow.Views;

public partial class ProductosView : ContentPage
{
    private bool _inicializado = false;

    public ProductosView(ProductosViewModel vm)
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
            if (BindingContext is ProductosViewModel vm)
            {
                await vm.InicializarAsync();
            }
        }
    }
}
