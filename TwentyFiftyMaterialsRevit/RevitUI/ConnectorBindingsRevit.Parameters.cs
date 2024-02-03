using Autodesk.Revit.DB;
using Autodesk.Revit.DB.ExtensibleStorage;
using Autodesk.Revit.UI;
using TwentyFiftyMaterialsCore.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Shapes;

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


        public void CreateElementParameters_Unwrapped()
        {
            // Create Element Parameters
            CategorySet categoryElementSet = CreateCategorySet(Utilities.SupportedBuiltInCategories);
            CreateParameters(categoryElementSet);
        }

        private void CreateParameters(CategorySet categorySet)
        {
            DefinitionFile sharedParameterFileDefinition = RevitApp.Application.OpenSharedParameterFile();

            DefinitionGroup definitionGroup = sharedParameterFileDefinition == null ? 
                null : sharedParameterFileDefinition.Groups.get_Item(AddOnName);

            if (sharedParameterFileDefinition == null || definitionGroup == null)
            {
                string assemblyLocation = Assembly.GetExecutingAssembly().Location;
                string assemblyDirectory = System.IO.Path.GetDirectoryName(assemblyLocation);
                string sharedParameterFilePath = assemblyDirectory + @"\2050Materials.txt";

                sharedParameterFileDefinition = SetAndOpenExternalSharedParamFile(sharedParameterFilePath);
            }

            definitionGroup = sharedParameterFileDefinition.Groups.get_Item(AddOnName);

            TFParameter<string> parameter = new TFParameter<string>();

            parameter.Name = "Test";
            parameter.Value = "Test";

            CreateStringParameter(parameter, definitionGroup.Definitions, (from def in definitionGroup.Definitions select def.Name).ToList());
        }
        private DefinitionFile SetAndOpenExternalSharedParamFile(string sharedParameterFilePath)
        {
            if (!File.Exists(sharedParameterFilePath))
            {
                CreateExternalSharedParamFile(sharedParameterFilePath);
            }

            // set the path of shared parameter file to current Revit
            RevitApp.Application.SharedParametersFilename = sharedParameterFilePath;

            // open the file
            return RevitApp.Application.OpenSharedParameterFile();

        }

        private void CreateExternalSharedParamFile(string sharedParameterFilePath)
        {
            FileStream fileStream = File.Create(sharedParameterFilePath);
            fileStream.Close();
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

        internal static string CreateStringParameter(TFParameter<string> parameter, Definitions parameterDefinitions, List<string> parameterDefinitionNames)
        {
            if (parameterDefinitionNames.Contains(parameter.Name)) return parameter.Name;// if the current definition exists

            ExternalDefinitionCreationOptions defCrOp = new ExternalDefinitionCreationOptions(parameter.Name, SpecTypeId.String.Text);

            // Set the visibility of the parameter
            defCrOp.Visible = parameter.Visible;

            var result = parameterDefinitions.Create(defCrOp);

            if (result == null)
            {
                throw new Exception("Parameter with name \"" + parameter.Name + "\" cannot be created.");
            }

            return parameter.Name;
        }

        internal static string CreateDoubleParameter(TFParameter<double> parameter, Definitions parameterDefinitions, List<string> parameterDefinitionNames)
        {
            if (parameterDefinitionNames.Contains(parameter.Name)) return parameter.Name;// if the current definition exists


            ExternalDefinitionCreationOptions defCrOp = new ExternalDefinitionCreationOptions(parameter.Name, SpecTypeId.Number);

            // Set the visibility of the parameter
            defCrOp.Visible = parameter.Visible;

            var result = parameterDefinitions.Create(defCrOp);

            if (result == null)
            {
                throw new Exception("Parameter with name \"" + parameter.Name + "\" cannot be created.");
            }

            return parameter.Name;
        }

        internal static string CreateIntegerParameter(TFParameter<int> parameter, Definitions parameterDefinitions, List<string> parameterDefinitionNames)
        {
            if (parameterDefinitionNames.Contains(parameter.Name)) return parameter.Name;// if the current definition exists


            ExternalDefinitionCreationOptions defCrOp = new ExternalDefinitionCreationOptions(parameter.Name, SpecTypeId.Int.Integer);

            // Set the visibility of the parameter
            defCrOp.Visible = parameter.Visible;

            var result = parameterDefinitions.Create(defCrOp);

            if (result == null)
            {
                throw new Exception("Parameter with name \"" + parameter.Name + "\" cannot be created.");
            }

            return parameter.Name;
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
