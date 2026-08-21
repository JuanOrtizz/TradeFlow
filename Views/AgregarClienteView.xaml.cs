using TradeFlow.ViewModels;

namespace TradeFlow.Views;

public partial class AgregarClienteView : ContentPage
{
    private bool _inicializado = false;

    public AgregarClienteView(AgregarClienteViewModel vm)
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
            if (BindingContext is AgregarClienteViewModel vm)
            {
                await vm.CargarLocalidadesAsync();
            }
        }
    }
}
