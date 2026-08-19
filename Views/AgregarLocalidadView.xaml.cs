using TradeFlow.ViewModels;

namespace TradeFlow.Views;

public partial class AgregarLocalidadView : ContentPage
{
    public AgregarLocalidadView(AgregarLocalidadViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }
}
