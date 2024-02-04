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


        public override void CreateElementParameters()
        {
            Queue.Add(new Action(() =>
            {
                using (Transaction t = new Transaction(CurrentDoc.Document, "Create Element Parameters"))
                {
                    t.Start();
                    CreateElementParameters_Unwrapped();
                    t.Commit();
                }
            }));
            Executor.Raise();
        }

        public void CreateElementParameters_Unwrapped()
        {
            // Create Element Parameters
            CategorySet categoryElementSet = CreateCategorySet(Utilities.SupportedBuiltInCategories);
            CreateParameters(categoryElementSet);
        }

        private void CreateParameters(CategorySet categorySet)
        {
            string assemblyLocation = Assembly.GetExecutingAssembly().Location;
            string assemblyDirectory = System.IO.Path.GetDirectoryName(assemblyLocation);
            string sharedParameterFilePath = assemblyDirectory + @"\2050Materials.txt";

            DefinitionFile sharedParameterFileDefinition = SetAndOpenExternalSharedParamFile(sharedParameterFilePath);

            DefinitionGroup definitionGroup = sharedParameterFileDefinition.Groups.get_Item(AddOnName);

            if (definitionGroup == null)
            {
                sharedParameterFileDefinition.Groups.Create(AddOnName);
                definitionGroup = sharedParameterFileDefinition.Groups.get_Item(AddOnName);
            }

            var groupNames = (from def in definitionGroup.Definitions select def.Name).ToList();

            CreateDoubleParameter("XXX_CO2_M_THICKNESS", definitionGroup.Definitions, groupNames);
            CreateStringParameter("XXX_CO2_M_NAME", definitionGroup.Definitions, groupNames);
            CreateStringParameter("XXX_CO2_M_UNITS", definitionGroup.Definitions, groupNames);
            CreateDoubleParameter("XXX_CO2_M_EMBODIEDCARBON", definitionGroup.Definitions, groupNames);

            foreach (Definition parameterDef in definitionGroup.Definitions)
            {
                if (CurrentDoc.Document.ParameterBindings.Contains(parameterDef)) continue;
                InstanceBinding instanceBinding = RevitApp.Application.Create.NewInstanceBinding(categorySet);


                // Get the BingdingMap of current document.
                BindingMap bindingMap = RevitApp.ActiveUIDocument.Document.ParameterBindings;

                // Bind the definitions to the document
                bool instanceBindOK = bindingMap.Insert(parameterDef,
                                                        instanceBinding,
                                                        BuiltInParameterGroup.PG_ANALYSIS_RESULTS);
            }
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

        internal static void CreateStringParameter(string parameter, Definitions parameterDefinitions, List<string> parameterDefinitionNames)
        {
            if (parameterDefinitionNames.Contains(parameter)) return;// if the current definition exists

            ExternalDefinitionCreationOptions defCrOp = new ExternalDefinitionCreationOptions(parameter, SpecTypeId.String.Text);

            // Set the visibility of the parameter
            defCrOp.Visible = true;

            var result = parameterDefinitions.Create(defCrOp);

            if (result == null)
            {
                throw new Exception("Parameter with name \"" + parameter + "\" cannot be created.");
            }

        }

        internal static void CreateDoubleParameter(string parameter, Definitions parameterDefinitions, List<string> parameterDefinitionNames)
        {
            if (parameterDefinitionNames.Contains(parameter)) return;// if the current definition exists


            ExternalDefinitionCreationOptions defCrOp = new ExternalDefinitionCreationOptions(parameter, SpecTypeId.Number);

            // Set the visibility of the parameter
            defCrOp.Visible = true;

            var result = parameterDefinitions.Create(defCrOp);

            if (result == null)
            {
                throw new Exception("Parameter with name \"" + parameter + "\" cannot be created.");
            }

        }

        internal static void CreateIntegerParameter(string parameter, Definitions parameterDefinitions, List<string> parameterDefinitionNames)
        {
            if (parameterDefinitionNames.Contains(parameter)) return;// if the current definition exists


            ExternalDefinitionCreationOptions defCrOp = new ExternalDefinitionCreationOptions(parameter, SpecTypeId.Int.Integer);

            // Set the visibility of the parameter
            defCrOp.Visible = true;

            var result = parameterDefinitions.Create(defCrOp);

            if (result == null)
            {
                throw new Exception("Parameter with name \"" + parameter + "\" cannot be created.");
            }

            return;
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
