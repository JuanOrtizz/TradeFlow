using System.ComponentModel;
using Microsoft.Maui.Handlers;
using Microsoft.UI.Input;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Input;
using WinUIButton = Microsoft.UI.Xaml.Controls.Button;

namespace TradeFlow.Platforms.Windows
{
    public class HandCursorButtonHandler : ButtonHandler
    {
        private sealed class HandButton : global::Microsoft.Maui.Platform.MauiButton
        {
            public void SetHandCursor(bool show) =>
                ProtectedCursor = show ? InputSystemCursor.Create(InputSystemCursorShape.Hand) : null;
        }

        protected override WinUIButton CreatePlatformView() => new HandButton();

        protected override void ConnectHandler(WinUIButton platformView)
        {
            base.ConnectHandler(platformView);

            platformView.PointerEntered += OnPointerEntered;
            platformView.PointerExited += OnPointerExited;
            platformView.PointerCanceled += OnPointerExited;

            if (VirtualView is Microsoft.Maui.Controls.VisualElement element)
                element.PropertyChanged += OnVirtualViewPropertyChanged;
        }

        protected override void DisconnectHandler(WinUIButton platformView)
        {
            if (VirtualView is Microsoft.Maui.Controls.VisualElement element)
                element.PropertyChanged -= OnVirtualViewPropertyChanged;

            platformView.PointerEntered -= OnPointerEntered;
            platformView.PointerExited -= OnPointerExited;
            platformView.PointerCanceled -= OnPointerExited;

            ((HandButton)platformView).SetHandCursor(false);
            base.DisconnectHandler(platformView);
        }

        private void OnVirtualViewPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(Microsoft.Maui.Controls.VisualElement.IsEnabled))
                UpdateCursor();
        }

        private void OnPointerEntered(object sender, PointerRoutedEventArgs e) => UpdateCursor();

        private void OnPointerExited(object sender, PointerRoutedEventArgs e) =>
            ((HandButton)sender).SetHandCursor(false);

        private void UpdateCursor()
        {
            if (PlatformView is HandButton button && VirtualView != null)
                button.SetHandCursor(VirtualView.IsEnabled);
        }
    }
}
