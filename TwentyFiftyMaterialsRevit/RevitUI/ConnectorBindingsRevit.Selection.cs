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
                    ApplyMaterialToElementProperties_Unwrapped();
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
                    ApplyMaterialToElementProperties_Unwrapped();
                    transaction.Commit();
                }

            }));
            Executor.Raise();
        }

        private void ApplyMaterialToElementProperties_Unwrapped()
        {
            ProjectModel.SelectedElements = new List<TFAssembly>();
            if (CurrentDoc == null) return;

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

                TFAssembly selectedElement = ProjectModel.SelectedAssembly.DeepClone();
                string key = element.UniqueId;

                selectedElement.Element_Guid = element.UniqueId;
                selectedElement.Element_Area = Utilities.ElementArea(element, CurrentDoc.Document);

                if(ProjectModel.ElementsWithAssemblies == null)
                {
                    ProjectModel.ElementsWithAssemblies = new Dictionary<string, TFAssembly>();
                }

                if (ProjectModel.ElementsWithAssemblies.Keys.Contains(key))
                {
                    ProjectModel.ElementsWithAssemblies[key] = selectedElement;
                }
                else
                {
                    ProjectModel.ElementsWithAssemblies.Add(selectedElement.Element_Guid, selectedElement);
                }

                ProjectModel.SelectedElements.Add(selectedElement);

                WriteElementParameter(element, "XXX_CO2_M_THICKNESS", selectedElement.XXX_CO2_M_THICKNESS, 5);
                WriteElementParameter(element, "XXX_CO2_M_NAME", selectedElement.XXX_CO2_M_NAME);
                WriteElementParameter(element, "XXX_CO2_M_UNITS", selectedElement.XXX_CO2_M_UNITS);
                WriteElementParameter(element, "XXX_CO2_M_EMBODIEDCARBON", selectedElement.XXX_CO2_M_EMBBODIEDCARBON, 5);
            }
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
