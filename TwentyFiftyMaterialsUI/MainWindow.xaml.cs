using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
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
using TwentyFiftyMaterialsCore.Models;
using TwentyFiftyMaterialsUI.Utils;

namespace TwentyFiftyMaterialsUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, IDisposable
    {
        public static ConnectorBindings Bindings;

        public static CultureInfo ProjectCultureInfo = new CultureInfo("", true);

        public MainWindow()
        {
            Bindings = new DummyBindings();
            Bindings.InitialiseUI(this);

            Closed += MainWindow_Closed;

            InitializeComponent();
        }

        public MainWindow(ConnectorBindings bindings)
        {
            Bindings = bindings;
            Bindings.InitialiseUI(this);

            Closed += MainWindow_Closed;

            InitializeComponent();
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
            ProjectModel.Initialize();
            PopulateAssembliesComboBox();
        }

        private void closeBtn_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void minimiseBtn_Click(object sender, RoutedEventArgs e)
        {
            if (this.ShowInTaskbar) this.WindowState = WindowState.Minimized;
        }

        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Materials_MouseDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }


        private void addMaterial_Click(object sender, RoutedEventArgs e)
        {
            Bindings.AddMaterial();
        }
        private void applyToSelection_Click(object sender, RoutedEventArgs e)
        {
            Bindings.ApplyAssemblyToSelectedElement();
        }

        private void PopulateAssembliesComboBox()
        {


            AssembliesComboBox.ItemsSource = ProjectModel.MaterialAssembies;
            AssembliesComboBox.DisplayMemberPath = "Name";
        }

        private void AssembliesComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox combobox = (ComboBox)sender;
            TFAssembly assembly = combobox.SelectedItem as TFAssembly;
            ProjectModel.SelectedAssembly = assembly;
        }
    }
}