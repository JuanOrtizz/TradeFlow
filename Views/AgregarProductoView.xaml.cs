using TradeFlow.ViewModels;

namespace TradeFlow.Views;

public partial class AgregarProductoView : ContentPage
{
    public AgregarProductoView(AgregarProductoViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }
}
