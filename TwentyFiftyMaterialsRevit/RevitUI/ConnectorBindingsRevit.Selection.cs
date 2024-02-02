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

        public override void UpdateModifiedElements()
        {

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
