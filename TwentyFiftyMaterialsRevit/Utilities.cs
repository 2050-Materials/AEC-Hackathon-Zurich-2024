using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using TwentyFiftyMaterialsRevit.RevitUI;

namespace TwentyFiftyMaterialsRevit
{
    internal class Utilities
    {
        internal static List<BuiltInCategory> SupportedBuiltInCategories = new List<BuiltInCategory>()
        {
            BuiltInCategory.OST_Floors,
            BuiltInCategory.OST_Walls,
            BuiltInCategory.OST_Roofs,
            BuiltInCategory.OST_Stairs,
            BuiltInCategory.OST_Windows,
            BuiltInCategory.OST_Doors,
            BuiltInCategory.OST_GenericModel,
            BuiltInCategory.OST_Ceilings,
            BuiltInCategory.OST_StructuralFoundation
        };

        public static int HashLength { get; } = 32;

        public enum HashingFuctions
        {
            SHA256, MD5
        }

        static string sha256(string input)
        {
            using (System.IO.MemoryStream ms = new System.IO.MemoryStream())
            {
                new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter().Serialize(ms, input);
                using (SHA256 sha = SHA256.Create())
                {
                    var hash = sha.ComputeHash(ms.ToArray());
                    StringBuilder sb = new StringBuilder();
                    foreach (byte b in hash)
                    {
                        sb.Append(b.ToString("X2"));
                    }

                    return sb.ToString().ToLower();
                }
            }
        }

        static string md5(string input)
        {
            using (System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5.Create())
            {
                byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(input.ToLowerInvariant());
                byte[] hashBytes = md5.ComputeHash(inputBytes);

                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < hashBytes.Length; i++)
                {
                    sb.Append(hashBytes[i].ToString("X2"));
                }
                return sb.ToString().ToLower();
            }
        }

        /// <summary>
        /// Wrapper method around hashing functions. Defaults to md5.
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string HashString(string input, HashingFuctions func = HashingFuctions.SHA256)
        {
            switch (func)
            {
                case HashingFuctions.SHA256:
                    return Utilities.sha256(input).Substring(0, HashLength);

                case HashingFuctions.MD5:
                default:
                    return Utilities.md5(input).Substring(0, HashLength);

            }
        }

        /// <summary>
        /// Method for collecting element types of a given category
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="category"></param>
        /// <returns></returns>
        internal static IList<FillPatternElement> CollectFillPatternTypes(Document doc)
        {
            FilteredElementCollector collector = new FilteredElementCollector(doc);
            IList<FillPatternElement> collectedElements = collector.OfClass(typeof(FillPatternElement)).Cast<FillPatternElement>().ToList();
            return collectedElements;
        }
        /// <summary>
        /// Get the Solid Fill Pattern
        /// </summary>
        /// <param name="doc"></param>
        /// <returns></returns>
        internal static FillPatternElement GetFillPattern(Document doc)
        {
            FillPatternElement solidFill = null;
            CollectFillPatternTypes(doc).ToList().ForEach(pattern => { if (pattern.GetFillPattern().IsSolidFill) solidFill = pattern; });
            return solidFill;
        }
        /// <summary>
        /// Method for collecting element instances of a given category
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="category"></param>
        /// <returns></returns>
        public static IList<Element> CollectElementInstances(Document doc, BuiltInCategory category)
        {
            FilteredElementCollector collector = new FilteredElementCollector(doc);
            IList<Element> collectedElements = collector.OfCategory(category).
                WhereElementIsNotElementType().ToElements();
            return collectedElements;
        }
        /// <summary>
        /// Method for collecting element types of a given category
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="category"></param>
        /// <returns></returns>
        public static IList<Element> CollectElementTypes(Document doc, BuiltInCategory category)
        {
            FilteredElementCollector collector = new FilteredElementCollector(doc);
            IList<Element> collectedElements = collector.OfCategory(category).
                WhereElementIsElementType().ToElements();
            return collectedElements;
        }
        /// <summary>
        /// Collect Instances from multiple categories.
        /// </summary>
        /// <returns></returns>
        internal static List<Element> InstancesFromAllSpecifiedCategories(Document doc)
        {
            List<Element> elements = new List<Element>();
            // Collect all elements from the specified categories
            foreach (BuiltInCategory category in Utilities.ParameterCategories) elements.AddRange(Utilities.CollectElementInstances(doc, category).ToList());
            return elements;
        }

        #region Unit Conversion
        internal static double ConvertToSqm(double sqft)
        {
            return UnitUtils.Convert(sqft, UnitTypeId.SquareFeet, UnitTypeId.SquareMeters);
        }
        internal static double ConvertToSqft(double sqm)
        {
            return UnitUtils.Convert(sqm, UnitTypeId.SquareMeters, UnitTypeId.SquareFeet);
        }
        internal static double ConvertToM(double ft)
        {
            return UnitUtils.Convert(ft, UnitTypeId.Feet, UnitTypeId.Meters);
        }
        internal static double ConvertToFt(double m)
        {
            return UnitUtils.Convert(m, UnitTypeId.Meters, UnitTypeId.Feet);
        }
        #endregion

        #region Elements 
        internal static List<BuiltInCategory> ParameterCategories = new List<BuiltInCategory>()
        {
            BuiltInCategory.OST_Floors,
            BuiltInCategory.OST_Walls,
            BuiltInCategory.OST_Roofs,
            BuiltInCategory.OST_Stairs,
            BuiltInCategory.OST_Windows,
            BuiltInCategory.OST_Doors,
            BuiltInCategory.OST_GenericModel,
            BuiltInCategory.OST_Ceilings,
            BuiltInCategory.OST_StructuralFoundation // Check geometry 
        };

        public static CultureInfo ProjectCulture { get; private set; }
        public static CultureInfo LocalCulture { get; private set; }

        public static List<ElementId> SelectedElementIds = new List<ElementId>();
        public static List<ElementId> SelectedAllowedElementIds = new List<ElementId>();
        public static bool UnsupportedCategorySelected = false;

        #endregion

        internal static double ParseLocalString(string valueStr)
        {
            double val;
            double.TryParse(valueStr, NumberStyles.Any, LocalCulture, out val);
            return val;
        }

        internal static double StringToDouble(string valueStr)
        {

            double val;
            double.TryParse(valueStr, NumberStyles.Any, ProjectCulture, out val);
            return val;
        }

        internal static string DoubleToString(double value, int digits)
        {
            string format = "0.";
            for (int i = 0; i < digits; i++) format += "0";
            string valueStr = Math.Round(value, digits).ToString(format, ProjectCulture);
            return valueStr;
        }

        internal static void GetProjectNumberFormatOptions(Document doc)
        {
            Units units = doc.GetUnits();

            CultureInfo projectCulture = new CultureInfo("", true);
            CultureInfo localCulture = System.Windows.Forms.Application.CurrentCulture;

            if (units.DecimalSymbol == DecimalSymbol.Dot)
            {
                projectCulture.NumberFormat.NumberDecimalSeparator = ".";
            }
            else if (units.DecimalSymbol == DecimalSymbol.Comma)
            {
                projectCulture.NumberFormat.NumberDecimalSeparator = ",";
            }

            if (units.DigitGroupingSymbol == DigitGroupingSymbol.Apostrophe)
            {
                projectCulture.NumberFormat.NumberGroupSeparator = "'";
            }
            else if (units.DigitGroupingSymbol == DigitGroupingSymbol.Comma)
            {
                projectCulture.NumberFormat.NumberGroupSeparator = ",";
            }
            else if (units.DigitGroupingSymbol == DigitGroupingSymbol.Dot)
            {
                projectCulture.NumberFormat.NumberGroupSeparator = ".";
            }
            else if (units.DigitGroupingSymbol == DigitGroupingSymbol.Space)
            {
                projectCulture.NumberFormat.NumberGroupSeparator = " ";
            }
            else if (units.DigitGroupingSymbol == DigitGroupingSymbol.Tick)
            {
                projectCulture.NumberFormat.NumberGroupSeparator = "'";
            }

            if (units.DigitGroupingAmount == DigitGroupingAmount.Two)
            {
                int[] size = { 2 };
                projectCulture.NumberFormat.NumberGroupSizes = size;
            }
            else if (units.DigitGroupingAmount == DigitGroupingAmount.Three)
            {
                int[] size = { 3 };
                projectCulture.NumberFormat.NumberGroupSizes = size;
            }

            ProjectCulture = projectCulture;
            LocalCulture = localCulture;
        }


        /// <summary>
        /// In meters
        /// </summary>
        /// <param name="element"></param>
        /// <param name="doc"></param>
        /// <returns></returns>
        internal static double ElementArea(Element element, Document doc)
        {
            double elementArea = 0;

            BuiltInCategory elementCategory = (BuiltInCategory)element.Category.Id.IntegerValue;
            if (elementCategory == BuiltInCategory.OST_Walls)//(element.GetType() == typeof(Wall))
            {
                elementArea = ComputedArea(element);
                if (elementArea == 0)
                {
                    elementArea = GeneralArea(element);
                }
            }
            else if (elementCategory == BuiltInCategory.OST_Floors) //(element.GetType() == typeof(Floor))
            {
                elementArea = ComputedArea(element);
                if (elementArea == 0)
                {
                    elementArea = GeneralArea(element);
                }
            }
            else if (elementCategory == BuiltInCategory.OST_Roofs)
            {
                elementArea = ComputedArea(element);
                if (elementArea == 0)
                {
                    elementArea = GeneralArea(element);
                }
            }
            else if (elementCategory == BuiltInCategory.OST_Windows || elementCategory == BuiltInCategory.OST_Doors)
            {
                elementArea = DoorWindowArea(element, doc);
                if (elementArea == 0)
                {
                    elementArea = ComputedArea(element);
                }
                if (elementArea == 0)
                {
                    elementArea = GeneralArea(element);
                }
            }
            else if (elementCategory == BuiltInCategory.OST_Stairs)
            {
                elementArea = GeneralArea(element);
            }
            else if (elementCategory == BuiltInCategory.OST_GenericModel)
            {
                elementArea = GeneralArea(element);
            }
            else if (elementCategory == BuiltInCategory.OST_Ceilings)
            {
                elementArea = ComputedArea(element);
            }
            else if (elementCategory == BuiltInCategory.OST_StructuralFoundation)
            {
                elementArea = ComputedArea(element);
                if (elementArea == 0)
                {
                    elementArea = GeneralArea(element);
                }

            }
            return elementArea;
        }

        internal static double GeneralArea(Element element)
        {

            double areaSqft = 0;
            ICollection<ElementId> materialIds = element.GetMaterialIds(false);

            if (materialIds.Count() == 0)
            {

                View3D view = (View3D)ConnectorBindingsRevit.CurrentDoc.Document.Views3D().FirstOrDefault(x => x.IsTemplate == false);
                if (view == null) return 0;

                var viewOptions = new Options()
                {
                    View = view
                };

                var elementGeometry = element.get_Geometry(viewOptions);
                foreach (GeometryObject geo in elementGeometry)
                {
                    Solid solid = geo as Solid;

                    if (solid != null)
                    {
                        areaSqft += solid.SurfaceArea;
                        continue;
                    }

                    GeometryInstance geoi = geo as GeometryInstance;

                    if (geoi != null)
                    {
                        GeometryElement instanceElement = geoi.GetInstanceGeometry();
                        foreach (GeometryObject instanceObject in instanceElement)
                        {
                            Solid instancesolid = instanceObject as Solid;
                            if (instancesolid != null)
                            {
                                areaSqft += instancesolid.SurfaceArea;
                            }
                        }
                        continue;
                    }
                }

                return Utilities.ConvertToSqm(areaSqft / 2);
            }

            foreach (ElementId id in materialIds)
            {
                areaSqft += element.GetMaterialArea(id, false);
            }

            return Utilities.ConvertToSqm(areaSqft / 2);
        }
        /// <summary>
        /// In meters
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        internal static double ComputedArea(Element element)
        {
            double areaSqft = 0;
            BuiltInParameter BuiltInAreaId = BuiltInParameter.HOST_AREA_COMPUTED;
            Parameter areaParameter = element.get_Parameter(BuiltInAreaId);
            if (areaParameter != null)
            {
                areaSqft = areaParameter.AsDouble();
            }

            return Utilities.ConvertToSqm(areaSqft);
        }
        /// <summary>
        /// In meters
        /// </summary>
        /// <param name="element"></param>
        /// <param name="doc"></param>
        /// <returns></returns>
        internal static double DoorWindowArea(Element element, Document doc)
        {
            double areaSqft = 0;

            Parameter elemHeight = element.get_Parameter(BuiltInParameter.DOOR_HEIGHT);
            Parameter elemWidth = element.get_Parameter(BuiltInParameter.DOOR_WIDTH);

            Parameter typeHeight = doc.GetElement(element.GetTypeId()).get_Parameter(BuiltInParameter.FAMILY_HEIGHT_PARAM);
            Parameter typeWidth = doc.GetElement(element.GetTypeId()).get_Parameter(BuiltInParameter.FAMILY_WIDTH_PARAM);

            if (elemHeight != null && elemHeight.AsDouble() != 0)
            {
                if (elemWidth != null && elemWidth.AsDouble() != 0)
                {
                    areaSqft = elemHeight.AsDouble() * elemWidth.AsDouble();
                }
                else if (typeWidth != null && typeWidth.AsDouble() != 0)
                {
                    areaSqft = elemHeight.AsDouble() * typeWidth.AsDouble();
                }
            }
            else if (typeHeight != null && typeHeight.AsDouble() != 0)
            {
                if (elemWidth != null && elemWidth.AsDouble() != 0)
                {
                    areaSqft = typeHeight.AsDouble() * elemWidth.AsDouble();
                }
                else if (typeWidth != null && typeWidth.AsDouble() != 0)
                {
                    areaSqft = typeHeight.AsDouble() * typeWidth.AsDouble();
                }
            }

            return Utilities.ConvertToSqm(areaSqft);
        }

        internal static List<ElementId> ExtractInteriorWallsFromDocument(Document doc)
        {
            List<ElementId> ids = new List<ElementId>();
            List<Element> elements = Utilities.CollectElementInstances(doc, BuiltInCategory.OST_Walls).ToList();
            foreach (Element element in elements)
            {
                WallType wallType = doc.GetElement(element.GetTypeId()) as WallType;
                if (wallType != null)
                {
                    if (wallType.Function == WallFunction.Interior) ids.Add(element.Id);
                }
            }
            return ids;
        }

    }
}
