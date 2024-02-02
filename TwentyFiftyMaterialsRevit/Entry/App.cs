using System;
using System.IO;
using System.Reflection;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Events;
using TwentyFiftyMaterialsRevit.RevitUI;
using Revit.Async;

namespace TwentyFiftyMaterialsRevit.Entry
{
    public class App : IExternalApplication
    {

        public static UIApplication AppInstance { get; set; }

        public static UIControlledApplication UICtrlApp { get; set; }

        public static PushButton UIStartButton { get; set; }

        static void AddRibbonPanel(UIControlledApplication application)
        {
            // Get dll assembly path
            string thisAssemblyPath = Assembly.GetExecutingAssembly().Location;
            string addOnVersion = Assembly.GetExecutingAssembly().GetName().Version.ToString();
            addOnVersion = addOnVersion.Substring(0, addOnVersion.Length - 2);
            string addOnName = "2050 Materials";

            RibbonPanel ribbonPanel;
            // Create a custom ribbon tab
            string tabName = addOnName;
            try
            {
                application.CreateRibbonTab(tabName);
            }
            catch { }

            // Add a new ribbon panel
            ribbonPanel = application.CreateRibbonPanel(tabName, $"2050 Materials");


            // create push button for CurveTotalLength
            PushButtonData b1Data = new PushButtonData(
                "cmd2050Materials",
                $"v{addOnVersion}",
                thisAssemblyPath,
                "TwentyFiftyMaterialsRevit.Entry.TwentyFiftyMaterialsCommand");

            PushButton pb1 = ribbonPanel.AddItem(b1Data) as PushButton;
            pb1.ToolTip = "Your Materials Copilot";
            BitmapImage pb1Image = new BitmapImage(new Uri("pack://application:,,,/TwentyFiftyMaterialsRevit;component/Resources/Logos/logo simple 32x32.ico"));
            pb1.LargeImage = pb1Image;

            UIStartButton = pb1;
        }

        public Result OnStartup(UIControlledApplication application)
        {
            //Always initialize RevitTask ahead of time within Revit API context
            RevitTask.Initialize(application);

            UICtrlApp = application;
            UICtrlApp.ControlledApplication.ApplicationInitialized += ControlledApplication_ApplicationInitialized;

            // call our method that will load up our toolbar
            AddRibbonPanel(application);

            return Result.Succeeded;
        }



        private void ControlledApplication_ApplicationInitialized(object sender, Autodesk.Revit.DB.Events.ApplicationInitializedEventArgs e)
        {

            AppInstance = new UIApplication(sender as Application);
            AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(OnAssemblyResolve);

            TwentyFiftyMaterialsCommand.Bindings = new ConnectorBindingsRevit(AppInstance);

            AppInstance.ViewActivated += RevitApp_ViewActivated;
            AppInstance.Application.DocumentClosed += Application_DocumentClosed;
            AppInstance.ViewActivated += AppInstance_ViewActivated;
            AppInstance.ViewActivating += AppInstance_ViewActivating;
        }

        public Result OnShutdown(UIControlledApplication application)
        {
            if (TwentyFiftyMaterialsCommand.Bindings.TFMDialog != null)
            {
                TwentyFiftyMaterialsCommand.Bindings.TFMDialog.Close();
                TwentyFiftyMaterialsCommand.Bindings = null;
            }
            return Result.Succeeded;
        }

        static Assembly OnAssemblyResolve(object sender, ResolveEventArgs args)
        {
            Assembly a = null;
            var name = args.Name.Split(',')[0];
            string path = Path.GetDirectoryName(typeof(App).Assembly.Location);

            string assemblyFile = Path.Combine(path, name + ".dll");

            if (File.Exists(assemblyFile))
                a = Assembly.LoadFrom(assemblyFile);

            return a;
        }

        private void AppInstance_ViewActivating(object sender, ViewActivatingEventArgs e)
        {
            if (e.CurrentActiveView == null) return;
            if (e.CurrentActiveView.Document == null) return;
            if (e.CurrentActiveView.Document.GetHashCode().Equals(e.NewActiveView.Document.GetHashCode())) return;
            if (TwentyFiftyMaterialsCommand.Bindings.TFMDialog == null) return;

            TwentyFiftyMaterialsCommand.Bindings.TFMDialog.DialogResult = false;
            TwentyFiftyMaterialsCommand.Bindings.TFMDialog.Close();
            TwentyFiftyMaterialsCommand.Bindings.TFMDialog = null;
        }

        private void AppInstance_ViewActivated(object sender, ViewActivatedEventArgs e)
        {
            if (Entry.App.UIStartButton != null)
            {
                if (e.Document.IsFamilyDocument)
                {
                    Entry.App.UIStartButton.Enabled = false;
                }
                else
                {
                    Entry.App.UIStartButton.Enabled = true;
                }
            }
        }

        private void RevitApp_ViewActivated(object sender, Autodesk.Revit.UI.Events.ViewActivatedEventArgs e)
        {
            if (e.Document == null || e.Document.IsFamilyDocument || e.PreviousActiveView == null || TwentyFiftyMaterialsCommand.Bindings.GetDocHash(e.Document) == TwentyFiftyMaterialsCommand.Bindings.GetDocHash(e.PreviousActiveView.Document))
                return;
            if (Entry.App.UIStartButton != null)
            {
                if (e.Document.IsFamilyDocument)
                {
                    Entry.App.UIStartButton.Enabled = false;
                }
                else
                {
                    Entry.App.UIStartButton.Enabled = true;
                }
            }
        }

        private void Application_DocumentClosed(object sender, Autodesk.Revit.DB.Events.DocumentClosedEventArgs e)
        {
            if (TwentyFiftyMaterialsCommand.Bindings == null) return;
            TwentyFiftyMaterialsCommand.Bindings.TFMDialog = null;
            if (TwentyFiftyMaterialsCommand.Bindings.TFMDialog == null) return;
            TwentyFiftyMaterialsCommand.Bindings.TFMDialog.Close();
        }
    }
}
