using TradeFlow.ViewModels;

namespace TradeFlow.Views;

public partial class BackupView : ContentPage
{
    public BackupView(BackupViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }
}
