using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Forms;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using TwentyFiftyMaterialsRevit.RevitUI;
using TwentyFiftyMaterialsUI;
using TwentyFiftyMaterialsCore;
using System.Windows.Data;
using System.Windows.Interop;

namespace TwentyFiftyMaterialsRevit.Entry
{
    [Transaction(TransactionMode.Manual)]
    public class TwentyFiftyMaterialsCommand : IExternalCommand
    {
        public static ConnectorBindingsRevit Bindings { get; set; }

        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            OpenOrFocusDialog(commandData.Application);
            return Result.Succeeded;
        }

        public static void OpenOrFocusDialog(UIApplication app)
        {
            if (Bindings.TFMDialog == null)
            {
                ExternalEventHandler eventHandler = new ExternalEventHandler(Bindings);
                ExternalEvent executor = ExternalEvent.Create(eventHandler);
                Bindings.SetExecutorAndInit(executor);
                Bindings.TFMDialog = new MainWindow(Bindings);
                WindowInteropHelper windowInteropHelper = new WindowInteropHelper(Bindings.TFMDialog);
                windowInteropHelper.Owner = app.MainWindowHandle;
                Bindings.TFMDialog.Show();
            }
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

        #region Messages
        #endregion
    }
}
