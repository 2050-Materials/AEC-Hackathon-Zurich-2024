using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TwentyFiftyMaterialsRevit
{
    public static class ConnectorRevitUtils
    {

        public static string RevitAppName = "Revit2024";

        private static List<string> _cachedParameters = null;
        private static List<string> _cachedViews = null;

        private static Dictionary<string, Category> _categories { get; set; }

        public static Dictionary<string, Category> GetCategories(Document doc)
        {
            if (_categories == null)
            {
                _categories = new Dictionary<string, Category>();
                foreach (var bic in SupportedBuiltInCategories)
                {
                    var category = Category.GetCategory(doc, bic);
                    if (category == null)
                        continue;
                    //some categories, in other languages (eg DEU) have duplicated names #542
                    if (_categories.ContainsKey(category.Name))
                    {
                        var spec = category.Id.ToString();
                        if (category.Parent != null)
                            spec = category.Parent.Name;
                        _categories.Add(category.Name + " (" + spec + ")", category);
                    }

                    else
                        _categories.Add(category.Name, category);
                }
            }
            return _categories;
        }

        #region extension methods
        public static List<Element> SupportedElements(this Document doc)
        {
            //get element types of supported categories
            var categoryFilter = new LogicalOrFilter(GetCategories(doc).Select(x => new ElementCategoryFilter(x.Value.Id)).Cast<ElementFilter>().ToList());

            List<Element> elements = new FilteredElementCollector(doc)
              .WhereElementIsNotElementType()
              .WhereElementIsViewIndependent()
              .WherePasses(categoryFilter).ToList();

            return elements;
        }

        public static List<Element> SupportedTypes(this Document doc)
        {
            //get element types of supported categories
            var categoryFilter = new LogicalOrFilter(GetCategories(doc).Select(x => new ElementCategoryFilter(x.Value.Id)).Cast<ElementFilter>().ToList());

            List<Element> elements = new FilteredElementCollector(doc)
              .WhereElementIsElementType()
              .WherePasses(categoryFilter).ToList();

            return elements;
        }

        public static List<View> Views2D(this Document doc)
        {
            List<View> views = new FilteredElementCollector(doc)
              .WhereElementIsNotElementType()
              .OfCategory(BuiltInCategory.OST_Views)
              .Cast<View>()
              .Where(x => x.ViewType == ViewType.CeilingPlan ||
              x.ViewType == ViewType.FloorPlan ||
              x.ViewType == ViewType.Elevation ||
              x.ViewType == ViewType.Section)
              .ToList();

            return views;
        }

        public static List<View> Views3D(this Document doc)
        {
            List<View> views = new FilteredElementCollector(doc)
              .WhereElementIsNotElementType()
              .OfCategory(BuiltInCategory.OST_Views)
              .Cast<View>()
              .Where(x => x.ViewType == ViewType.ThreeD)
              .ToList();

            return views;
        }

        public static List<Element> Levels(this Document doc)
        {
            List<Element> levels = new FilteredElementCollector(doc)
              .WhereElementIsNotElementType()
              .OfCategory(BuiltInCategory.OST_Levels).ToList();

            return levels;
        }
        #endregion

        public static List<string> GetCategoryNames(Document doc)
        {
            return GetCategories(doc).Keys.OrderBy(x => x).ToList();
        }

        public static List<string> GetWorksets(Document doc)
        {
            return new FilteredWorksetCollector(doc).Where(x => x.Kind == WorksetKind.UserWorkset).Select(x => x.Name).ToList();
        }

        private static async Task<List<string>> GetParameterNamesAsync(Document doc)
        {
            var els = new FilteredElementCollector(doc)
              .WhereElementIsNotElementType()
              .WhereElementIsViewIndependent()
              .Where(x => x.IsPhysicalElement());

            List<string> parameters = new List<string>();

            foreach (var e in els)
            {
                foreach (Parameter p in e.Parameters)
                {
                    if (!parameters.Contains(p.Definition.Name))
                        parameters.Add(p.Definition.Name);
                }
            }
            _cachedParameters = parameters.OrderBy(x => x).ToList();
            return _cachedParameters;
        }

        /// <summary>
        /// Each time it's called the cached parameters are returned, and a new copy is cached
        /// </summary>
        /// <param name="doc"></param>
        /// <returns></returns>
        public static List<string> GetParameterNames(Document doc)
        {
            if (_cachedParameters != null)
            {
                //don't wait for it to finish
                GetParameterNamesAsync(doc);
                return _cachedParameters;
            }
            return GetParameterNamesAsync(doc).Result;
        }

        private static async Task<List<string>> GetViewNamesAsync(Document doc)
        {
            var els = new FilteredElementCollector(doc)
              .WhereElementIsNotElementType()
              .OfClass(typeof(View))
              .ToElements();

            _cachedViews = els.Select(x => x.Name).OrderBy(x => x).ToList();
            return _cachedViews;
        }

        /// <summary>
        /// Each time it's called the cached parameters are return, and a new copy is cached
        /// </summary>
        /// <param name="doc"></param>
        /// <returns></returns>
        public static List<string> GetViewNames(Document doc)
        {
            if (_cachedViews != null)
            {
                //don't wait for it to finish
                GetViewNamesAsync(doc);
                return _cachedViews;
            }
            return GetViewNamesAsync(doc).Result;
        }

        public static bool IsPhysicalElement(this Element e)
        {
            if (e.Category == null) return false;
            if (e.ViewSpecific) return false;
            // exclude specific unwanted categories
            if (((BuiltInCategory)e.Category.Id.IntegerValue) == BuiltInCategory.OST_HVAC_Zones) return false;
            return e.Category.CategoryType == CategoryType.Model && e.Category.CanAddSubcategory;
        }

        public static bool IsElementSupported(this Element e)
        {
            if (e.Category == null) return false;
            if (e.ViewSpecific) return false;

            if (SupportedBuiltInCategories.Contains((BuiltInCategory)e.Category.Id.IntegerValue))
                return true;
            return false;
        }

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
    }
}
