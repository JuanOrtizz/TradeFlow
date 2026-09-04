using System.Windows.Input;
using Microsoft.Maui.Controls;

namespace TradeFlow.Helpers
{
    // Ejecuta un comando cuando el elemento asociado recibe el foco.
    // Uso en XAML:
    //   <Entry.Behaviors>
    //       <helpers:LimpiaErrorAlEnfocarBehavior Command="{Binding LimpiarErrorCommand}" CommandParameter="Nombre" />
    //   </Entry.Behaviors>
    public class LimpiaErrorAlEnfocarBehavior : Behavior<VisualElement>
    {
        public static readonly BindableProperty CommandProperty =
            BindableProperty.Create(nameof(Command), typeof(ICommand), typeof(LimpiaErrorAlEnfocarBehavior));

        public static readonly BindableProperty CommandParameterProperty =
            BindableProperty.Create(nameof(CommandParameter), typeof(object), typeof(LimpiaErrorAlEnfocarBehavior));

        private VisualElement? _elementoAsociado;

        public ICommand Command
        {
            get => (ICommand)GetValue(CommandProperty);
            set => SetValue(CommandProperty, value);
        }

        public object CommandParameter
        {
            get => GetValue(CommandParameterProperty);
            set => SetValue(CommandParameterProperty, value);
        }

        protected override void OnAttachedTo(VisualElement bindable)
        {
            base.OnAttachedTo(bindable);
            _elementoAsociado = bindable;
            bindable.BindingContextChanged += OnElementoBindingContextChanged;
            bindable.Focused += OnFocused;
            BindingContext = bindable.BindingContext;
        }

        protected override void OnDetachingFrom(VisualElement bindable)
        {
            base.OnDetachingFrom(bindable);
            bindable.BindingContextChanged -= OnElementoBindingContextChanged;
            bindable.Focused -= OnFocused;
            _elementoAsociado = null;
        }

        private void OnElementoBindingContextChanged(object? sender, EventArgs e)
        {
            if (_elementoAsociado != null)
            {
                BindingContext = _elementoAsociado.BindingContext;
            }
        }

        private void OnFocused(object? sender, FocusEventArgs e)
        {
            if (Command?.CanExecute(CommandParameter) == true)
            {
                Command.Execute(CommandParameter);
            }
        }
    }
}
