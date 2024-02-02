using Autodesk.Revit.DB;
using Autodesk.Revit.DB.ExtensibleStorage;
using Autodesk.Revit.UI;
using TwentyFiftyMaterialsCore.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace TwentyFiftyMaterialsRevit.RevitUI
{
    public partial class ConnectorBindingsRevit
    {

        #region Overrides

        public override void SetCurrentProject()
        {
            if (CurrentDoc == null) return;
            if (CurrentProject != null) return;
        }


        public void CreateBauteilParameters_Unwrapped()
        {
            // Create Element Parameters
            CategorySet categoryElementSet = CreateCategorySet(Utilities.SupportedBuiltInCategories);
            CreateParameters(categoryElementSet);
        }
        private void CreateParameters(CategorySet categorySet)
        {

        }
        public override void ReadProjectInfoParameters()
        {
 
        }


        #endregion

        internal static CategorySet CreateCategorySet(List<BuiltInCategory> categories)
        {
            CategorySet categorySet = RevitApp.Application.Create.NewCategorySet();
            foreach (BuiltInCategory builtInCategory in categories)
            {
                Category category = CurrentDoc.Document.Settings.Categories.get_Item(builtInCategory);
                if(category == null || CurrentDoc.Document.Settings.Categories.Contains(category.Name) == false)
                {
                    continue;
                }
                categorySet.Insert(category);
            }
            return categorySet;
        }

        internal static double ReadProjectDoubleParameter(string name)
        {
            return ProjectInformation.GetParameters(name)[0].AsDouble();
        }

        internal static string ReadProjectStringParameter(string name)
        {
            return ProjectInformation.GetParameters(name)[0].AsString();
        }

        internal static int ReadProjectIntegerParameter(string name)
        {
            return ProjectInformation.GetParameters(name)[0].AsInteger();
        }

        internal static double ReadElementDoubleParameter(Element element, string name)
        {
            double value = element.GetParameters(name)[0].AsDouble();
            return value;
        }

        internal static string ReadElementStringParameter(Element element, string name)
        {
            string value = string.Empty;
            IList <Parameter> parameters = element.GetParameters(name);
            if (CanEditParameters(parameters)) value = element.GetParameters(name)[0].AsString();
            return value;
        }

        internal static void WriteElementParameter(Element element, string name, string value)
        {

            IList<Parameter> parameters = element.GetParameters(name);
            if (CanEditParameters(parameters) && value != null) parameters[0].Set(value);

        }

        internal static void WriteElementParameter(Element element, string name, double value, int digits)
        {
            IList<Parameter> parameters = element.GetParameters(name);
            if (CanEditParameters(parameters) && !double.IsNaN(value)) parameters[0].Set(Math.Round(value, digits));

        }

        internal static void WriteElementParameter(Element element, string name, int value)
        {

            IList<Parameter> parameters = element.GetParameters(name);
            if (CanEditParameters(parameters))
            {
                parameters[0].Set(value);
            }
        }

        internal static void WriteProjectParameter(string name, double value, int digits)
        {
            IList<Parameter> parameters = ProjectInformation.GetParameters(name);
            if (CanEditParameters(parameters) && !double.IsNaN(value)) parameters[0].Set(Math.Round(value, digits));
        }

        internal static void WriteProjectParameter(string name, string value)
        {
            IList<Parameter> parameters = ProjectInformation.GetParameters(name);
            if (CanEditParameters(parameters) && value != null) parameters[0].Set(value);
        }

        internal static void WriteProjectParameter(string name, int value)
        {
            IList<Parameter> parameters = ProjectInformation.GetParameters(name);
            if(CanEditParameters(parameters)) parameters[0].Set(value);
        }

        internal static bool CanEditParameters(IList<Parameter> parameters)
        {
            if (parameters.Count == 0)
            {
                return false;
            }

            if (parameters[0].IsReadOnly)
            {
                return false;
            }

            return true;
        }

    }
}
