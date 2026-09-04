using System.ComponentModel;
using TradeFlow.ViewModels;

namespace TradeFlow.Views;

public partial class FacturasView : ContentPage
{
    public FacturasView(FacturasViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        if (BindingContext is FacturasViewModel vm)
        {
            await vm.InicializarAsync();
        }
    }

    private void OnFechaSeleccionada(object? sender, DateChangedEventArgs e)
    {
        if (BindingContext is FacturasViewModel vm)
        {
            vm.FechaSeleccionada = e.NewDate;
            vm.FiltraPorFecha = true;
            vm.BuscarCommand.Execute(null);
        }
    }

    private void OnHoyClicked(object sender, EventArgs e)
    {
        if (BindingContext is FacturasViewModel vm)
        {
            vm.FechaSeleccionada = DateTime.Today;
            vm.FiltraPorFecha = true;
            vm.BuscarCommand.Execute(null);
        }
    }

    private void OnTodasClicked(object sender, EventArgs e)
    {
        if (BindingContext is FacturasViewModel vm)
        {
            vm.FiltraPorFecha = false;
            vm.BuscarCommand.Execute(null);
        }
    }
}
