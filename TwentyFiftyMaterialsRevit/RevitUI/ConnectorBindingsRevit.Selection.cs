using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using TwentyFiftyMaterialsCore.Models;
using TwentyFiftyMaterialsCore;

namespace TwentyFiftyMaterialsRevit.RevitUI
{
    public partial class ConnectorBindingsRevit
    {
        public override void ApplyMaterialToSelection()
        {
            if (CurrentDoc == null) return;
            if (ProjectModel.SelectedMaterial == null) return;

            ICollection<ElementId> selectedElementIds = CurrentDoc.Selection.GetElementIds();

            foreach (ElementId elementId in selectedElementIds)
            {
                Element element = CurrentDoc.Document.GetElement(elementId);
                if (element.Category == null) continue;

                int elementCategory = (int)Utilities.ParameterCategories.Where(item => (int)item == element.Category.Id.IntegerValue).FirstOrDefault();

                // If seleted element is not in allowed categories no parameters are written
                if (elementCategory == 0)
                {
                    continue;
                }
            }
        }


        public override void ApplyAssemblyToSelectedElement()
        {
            Queue.Add(new Action(() =>
            {
                using (Transaction transaction = new Transaction(CurrentDoc.Document, "Apply Material to Selected Eleent"))
                {
                    transaction.Start();
                    AppltMaterialToElementProperties_Unwrapped();
                    transaction.Commit();
                }

            }));
            Executor.Raise();
        }
        public override void AppltMaterialToElementProperties()
        {
            Queue.Add(new Action(() =>
            {
                using (Transaction transaction = new Transaction(CurrentDoc.Document, "Apply Material to Selected Eleent"))
                {
                    transaction.Start();
                    AppltMaterialToElementProperties_Unwrapped();
                    transaction.Commit();
                }

            }));
            Executor.Raise();
        }

        private void AppltMaterialToElementProperties_Unwrapped()
        {
            throw new NotImplementedException();
        }

        public override List<string> GetObjectsInView()
        {
            if (CurrentDoc == null)
            {
                return new List<string>();
            }

            var collector = new FilteredElementCollector(CurrentDoc.Document, CurrentDoc.Document.ActiveView.Id).WhereElementIsNotElementType();
            var elementIds = collector.ToElements().Select(el => el.UniqueId).ToList(); 

            return elementIds;
        }

        private string GetStringValue(Parameter p)
        {
            string value = "";
            if (!p.HasValue)
            {
                return value;
            }

            if (string.IsNullOrEmpty(p.AsValueString()) && string.IsNullOrEmpty(p.AsString()))
            {
                return value;
            }

            if (!string.IsNullOrEmpty(p.AsValueString()))
            {
                return p.AsValueString().ToLowerInvariant();
            }
            else
            {
                return p.AsString().ToLowerInvariant();
            }
        }

    }
}
