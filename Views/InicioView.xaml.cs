using TradeFlow.ViewModels;

namespace TradeFlow.Views;

public partial class InicioView : ContentPage
{
    public InicioView(InicioViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        if (BindingContext is InicioViewModel vm)
        {
            await vm.InicializarAsync();
        }
    }
}
