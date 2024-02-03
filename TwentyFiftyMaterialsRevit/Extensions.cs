using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TwentyFiftyMaterialsRevit
{
    public static class Extensions
    {
        public static FilteredElementCollector WhereParameterEqualsTo(this FilteredElementCollector collector, BuiltInParameter paramId, int value)
        {
            using (var provider = new ParameterValueProvider(new ElementId(paramId)))
            using (var evaluator = new FilterNumericEquals())
            using (var rule = new FilterIntegerRule(provider, evaluator, value))
            using (var filter = new ElementParameterFilter(rule))
                return collector.WherePasses(filter);
        }

        public static FilteredElementCollector WhereParameterEqualsTo(this FilteredElementCollector collector, BuiltInParameter paramId, string value)
        {
            if (value is null) return collector;

            using (var provider = new ParameterValueProvider(new ElementId(paramId)))
            using (var evaluator = new FilterStringEquals())
            using (var rule = new FilterStringRule(provider, evaluator, value))
            using (var filter = new ElementParameterFilter(rule))
                return collector.WherePasses(filter);
        }

        public static FilteredElementCollector WhereParameterEqualsTo(this FilteredElementCollector collector, BuiltInParameter paramId, ElementId value)
        {
            if (value is null) return collector;

            using (var provider = new ParameterValueProvider(new ElementId(paramId)))
            using (var evaluator = new FilterNumericEquals())
            using (var rule = new FilterElementIdRule(provider, evaluator, value))
            using (var filter = new ElementParameterFilter(rule))
                return collector.WherePasses(filter);
        }
    }
}
