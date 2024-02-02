using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Timers;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Events;
using TwentyFiftyMaterialsUI;
using TwentyFiftyMaterialsCore.Models;

namespace TwentyFiftyMaterialsRevit.RevitUI
{
    public partial class ConnectorBindingsRevit : ConnectorBindings
    {
       
        public static UIApplication RevitApp;

        public static UIDocument CurrentDoc => RevitApp.ActiveUIDocument;

        public static ProjectInfo ProjectInformation => Utilities.CollectElementInstances(CurrentDoc.Document, BuiltInCategory.OST_ProjectInformation).Cast<ProjectInfo>().ToList()[0];
        public static List<ElementId> NewElementIds = new List<ElementId>();


        /// <summary>
        /// Stores the actions for the ExternalEvent handler
        /// </summary>
        public List<Action> Queue;

        public ExternalEvent Executor;


        public ConnectorBindingsRevit(UIApplication revitApp) : base()
        {
            RevitApp = revitApp;
            Queue = new List<Action>();
        }


        /// <summary>
        /// Sets the revit external event handler and initialises the rocket engines.
        /// </summary>
        /// <param name="executor"></param>
        public void SetExecutorAndInit(ExternalEvent executor)
        {
            Executor = executor;
        }

        public override void DisposeExecutor()
        {
            // we own both the event and the handler
            // we should dispose it before we are closed
            Executor.Dispose();
            Executor = null;
        }

        private void AppInstance_ViewActivating(object sender, ViewActivatingEventArgs e)
        {
            if (TFMDialog == null) return;
            if (e.CurrentActiveView == null) return;
            if (e.CurrentActiveView.Document.GetHashCode().Equals(e.NewActiveView.Document.GetHashCode()) == false)
            {
                TFMDialog.DialogResult = false;
                TFMDialog.Close();
            }
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
        /// <summary>
        /// Get Current selection right now.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SelectionTimer_Elapsed(object sender, ElapsedEventArgs e)
        {

        }

        public override string GetHostAppName() => ConnectorRevitUtils.RevitAppName.Replace("Revit", "Revit "); //hack for ADSK store

        public override string GetDocumentId() => GetDocHash(CurrentDoc?.Document);

        public string GetDocHash(Document doc) => Utilities.HashString(doc.PathName + doc.Title, Utilities.HashingFuctions.MD5);

        public override string GetDocumentLocation() => CurrentDoc.Document.PathName;

        public override string GetActiveViewName() => CurrentDoc.Document.ActiveView.Title;

        public override string GetFileName() => CurrentDoc.Document.Title;


        #region app events

        //checks whether to refresh the stream list in case the user changes active view and selects a different document
        private void RevitApp_ViewActivated(object sender, Autodesk.Revit.UI.Events.ViewActivatedEventArgs e)
        {
            if (e.Document == null || e.Document.IsFamilyDocument || e.PreviousActiveView == null || GetDocHash(e.Document) == GetDocHash(e.PreviousActiveView.Document))
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
            if (TFMDialog == null) return;
            TFMDialog.Close();
        }

        private void Application_DocumentOpened(object sender, Autodesk.Revit.DB.Events.DocumentOpenedEventArgs e)
        {
            if (CurrentDoc == null) return;
            //CurrentProject = ProjectSettingsManager.ReadProjectModel(CurrentDoc.Document);
            //GetSelectionFilter();
        }

        #endregion

        public override string GetProjectNameFromApplication()
        {
            string projectName = CurrentDoc.Document.ProjectInformation.Name;
            if (string.IsNullOrWhiteSpace(projectName)) return "";
            return projectName;
        }

        public override List<string> GetProjectCultureInfo()
        {
            Units units = CurrentDoc.Document.GetUnits();

            List<string> projectCulture = new List<string>();

            if (units.DecimalSymbol == DecimalSymbol.Dot)
            {
                projectCulture.Add(".");
            }
            else if (units.DecimalSymbol == DecimalSymbol.Comma)
            {
                projectCulture.Add(",");
            }

            if (units.DigitGroupingSymbol == DigitGroupingSymbol.Apostrophe)
            {
                projectCulture.Add("'");
            }
            else if (units.DigitGroupingSymbol == DigitGroupingSymbol.Comma)
            {
                projectCulture.Add(",");
            }
            else if (units.DigitGroupingSymbol == DigitGroupingSymbol.Dot)
            {
                projectCulture.Add(".");
            }
            else if (units.DigitGroupingSymbol == DigitGroupingSymbol.Space)
            {
                projectCulture.Add(" ");
            }
            else if (units.DigitGroupingSymbol == DigitGroupingSymbol.Tick)
            {
                projectCulture.Add("'");
            }

            if (units.DigitGroupingAmount == DigitGroupingAmount.Two)
            {
                int[] size = { 2 };
                
                projectCulture.Add(2.ToString());
            }
            else if (units.DigitGroupingAmount == DigitGroupingAmount.Three)
            {
                int[] size = { 3 };
                projectCulture.Add(3.ToString());
            }

            return projectCulture;
        }


    }
}
