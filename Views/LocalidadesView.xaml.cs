using TradeFlow.ViewModels;

namespace TradeFlow.Views;

public partial class LocalidadesView : ContentPage
{
    public LocalidadesView(LocalidadesViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        if (BindingContext is LocalidadesViewModel vm)
        {
            await vm.InicializarAsync();
        }
    }
}
