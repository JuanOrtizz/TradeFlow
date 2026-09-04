using System.Reflection;
using Microsoft.Maui.Controls;

namespace TradeFlow.Helpers
{
    // Propiedad adjunta para mostrar el cursor de mano al pasar el mouse sobre un elemento.
    // Uso en XAML: helpers:HandCursor.IsEnabled="True"
    //
    // MAUI no tiene API para cambiar el cursor, asi que en Windows se setea
    // ProtectedCursor del control nativo via reflexion (la propiedad es protected).
    // En el resto de plataformas es un no-op (no hay mouse).
    public static class HandCursor
    {
        public static readonly BindableProperty IsEnabledProperty =
            BindableProperty.CreateAttached(
                "IsEnabled",
                typeof(bool),
                typeof(HandCursor),
                false,
                propertyChanged: OnIsEnabledChanged);

        public static bool GetIsEnabled(BindableObject view)
            => (bool)view.GetValue(IsEnabledProperty);

        public static void SetIsEnabled(BindableObject view, bool value)
            => view.SetValue(IsEnabledProperty, value);

        private static void OnIsEnabledChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if (bindable is not View element || !(bool)newValue)
                return;

            var gesture = new PointerGestureRecognizer();
            gesture.PointerEntered += (s, e) => SetCursor(element, CrearCursorMano());
            gesture.PointerExited += (s, e) => SetCursor(element, null);
            element.GestureRecognizers.Add(gesture);
        }

#if WINDOWS
        private static object CrearCursorMano()
            => Microsoft.UI.Input.InputSystemCursor.Create(Microsoft.UI.Input.InputSystemCursorShape.Hand);

        private static void SetCursor(VisualElement element, object? cursor)
        {
            if (element.Handler?.PlatformView is not Microsoft.UI.Xaml.FrameworkElement fe)
                return;

            typeof(Microsoft.UI.Xaml.UIElement)
                .GetProperty("ProtectedCursor", BindingFlags.NonPublic | BindingFlags.Instance)
                ?.SetValue(fe, cursor);
        }
#else
        private static object? CrearCursorMano() => null;

        private static void SetCursor(VisualElement element, object? cursor)
        {
        }
#endif
    }
}
