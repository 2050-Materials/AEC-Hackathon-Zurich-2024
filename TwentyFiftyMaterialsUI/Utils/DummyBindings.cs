using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TwentyFiftyMaterialsUI.Utils
{
    public class DummyBindings : ConnectorBindings
    {
        public DummyBindings() : base() { }

        public override void AddMaterial()
        {
            return;
        }

        public override void ApplyMaterialToSelection()
        {
            return;
        }

        public override void DisposeExecutor()
        {
            return;
        }

        public override string GetActiveViewName()
        {
            return " ";
        }

        public override string GetDocumentId()
        {
            return " ";
        }

        public override string GetDocumentLocation()
        {
            return " ";
        }

        public override string GetFileName()
        {
            return " ";
        }

        public override string GetHostAppName()
        {
            return " ";
        }

        public override List<string> GetObjectsInView()
        {
            return new List<string>();
        }

        public override List<string> GetProjectCultureInfo()
        {
            CultureInfo currentCulture = new CultureInfo("", true);
            List<string> projectCultureInfo = new List<string>();
            projectCultureInfo.Add(currentCulture.NumberFormat.NumberDecimalSeparator);
            projectCultureInfo.Add(currentCulture.NumberFormat.NumberGroupSeparator);
            projectCultureInfo.Add(currentCulture.NumberFormat.NumberGroupSizes[0].ToString());
            return projectCultureInfo;
        }

        public override string GetProjectNameFromApplication()
        {
            return "Default Project Name";
        }

        public override void ReadProjectInfoParameters()
        {
            return;
        }

        public override void SetCurrentProject()
        {
            return;
        }

        public override void AppltMaterialToElementProperties()
        {
            return;
        }

        public override void ApplyAssemblyToSelectedElement()
        {
            throw new NotImplementedException();
        }
    }
}
