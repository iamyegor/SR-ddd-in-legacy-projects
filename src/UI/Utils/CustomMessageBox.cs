using System.Windows;

namespace PackageDelivery.Utils;

public class CustomMessageBox
{
    public static void ShowError(string errorMesssage)
    {
        string caption = "Warning";
        MessageBoxButton button = MessageBoxButton.OK;
        MessageBoxImage icon = MessageBoxImage.Warning;

        MessageBox.Show(errorMesssage, caption, button, icon);
    }
}
