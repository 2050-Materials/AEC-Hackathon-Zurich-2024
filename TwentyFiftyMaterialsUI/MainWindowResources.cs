using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace TwentyFiftyMaterialsUI
{
    public partial class MainWindowResources : ResourceDictionary
    {
        private void closeBtn_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.Bindings.TFMDialog.Close();
        }

        private void minimiseBtn_Click(object sender, RoutedEventArgs e)
        {
            if (MainWindow.Bindings.TFMDialog.ShowInTaskbar) MainWindow.Bindings.TFMDialog.WindowState = WindowState.Minimized;
        }

        private void windowTab_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            MainWindow.Bindings.TFMDialog.DragMove();
        }
    }
}
