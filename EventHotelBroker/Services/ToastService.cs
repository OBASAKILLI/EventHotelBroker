using System;

namespace EventHotelBroker.Services
{
    public class ToastService
    {
        public event Action<string, string, ToastType>? OnShow;

        public void ShowSuccess(string title, string message)
        {
            OnShow?.Invoke(title, message, ToastType.Success);
        }

        public void ShowError(string title, string message)
        {
            OnShow?.Invoke(title, message, ToastType.Error);
        }

        public void ShowWarning(string title, string message)
        {
            OnShow?.Invoke(title, message, ToastType.Warning);
        }

        public void ShowInfo(string title, string message)
        {
            OnShow?.Invoke(title, message, ToastType.Info);
        }
    }

    public enum ToastType
    {
        Success,
        Error,
        Warning,
        Info
    }
}
