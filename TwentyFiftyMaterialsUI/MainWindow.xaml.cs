using System;
using System.Globalization;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace TwentyFiftyMaterialsUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, IDisposable
    {
        public static ConnectorBindings Bindings;

        public static CultureInfo ProjectCultureInfo = new CultureInfo("", true);

        public MainWindow(ConnectorBindings bindings)
        {
            Bindings = bindings;
            Bindings.InitialiseUI(this);

            Closed += MainWindow_Closed;
            Deactivated += MainWindow_Deactivated;

            InitializeComponent();
        }

        private void MainWindow_Deactivated(object sender, EventArgs e)
        {
            Window window = (Window)sender;
            //if (window..Topmost)
            //{
            //    window.Topmost = true;
            //}

        }

        /// <summary>
        /// Convert WPF to IDisposable
        /// </summary>
        public void Dispose()
        {
            Close();
            Bindings.TFMDialog = null;
        }

        private void MainWindow_Closed(object sender, EventArgs e)
        {
            // we own both the event and the handler
            // we should dispose it before we are closed
            Bindings.DisposeExecutor();

            // do not forget to call the base class
            Dispose();
        }

        //Handles the Load event of the SpiralLayoutForm Load
        private void UI_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
            this.Close();
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }
    }
}