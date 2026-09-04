using System.Windows.Input;
using Microsoft.Maui.Controls;

namespace TradeFlow.Helpers
{
    // Ejecuta un comando cuando el elemento asociado pierde el foco.
    // Uso en XAML:
    //   <Entry.Behaviors>
    //       <helpers:EjecutarComandoAlDesenfocarBehavior Command="{Binding OcultarSugerenciasCommand}" />
    //   </Entry.Behaviors>
    public class EjecutarComandoAlDesenfocarBehavior : Behavior<VisualElement>
    {
        public static readonly BindableProperty CommandProperty =
            BindableProperty.Create(nameof(Command), typeof(ICommand), typeof(EjecutarComandoAlDesenfocarBehavior));

        private VisualElement? _elementoAsociado;

        public ICommand Command
        {
            get => (ICommand)GetValue(CommandProperty);
            set => SetValue(CommandProperty, value);
        }

        protected override void OnAttachedTo(VisualElement bindable)
        {
            base.OnAttachedTo(bindable);
            _elementoAsociado = bindable;
            bindable.BindingContextChanged += OnElementoBindingContextChanged;
            bindable.Unfocused += OnUnfocused;
            BindingContext = bindable.BindingContext;
        }

        protected override void OnDetachingFrom(VisualElement bindable)
        {
            base.OnDetachingFrom(bindable);
            bindable.BindingContextChanged -= OnElementoBindingContextChanged;
            bindable.Unfocused -= OnUnfocused;
            _elementoAsociado = null;
        }

        private void OnElementoBindingContextChanged(object? sender, EventArgs e)
        {
            if (_elementoAsociado != null)
            {
                BindingContext = _elementoAsociado.BindingContext;
            }
        }

        private void OnUnfocused(object? sender, FocusEventArgs e)
        {
            if (Command?.CanExecute(null) == true)
            {
                Command.Execute(null);
            }
        }
    }
}
