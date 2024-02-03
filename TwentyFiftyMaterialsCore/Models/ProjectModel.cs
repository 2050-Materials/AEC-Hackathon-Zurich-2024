using System;
using System.Collections.Generic;
using System.Text;

namespace TwentyFiftyMaterialsCore.Models
{
    public class ProjectModel
    {
        public string Token { get; set; }

        public static TFMaterial SelectedMaterial { get; set; }
        public static TFAssembly SelectedAssembly { get; set; }
        public static List<TFAssembly> MaterialAssembies { get; set; }
        public static List<TFAssembly> SelectedElements { get; set; }
        public static Dictionary<string, TFAssembly> ElementsWithAssemblies { get; set; }

        public static void PopulateAssemblies()
        {         
            MaterialAssembies = ReadExcel.ReadExcelTable();

            //Dummy example for populating assemblies
            //MaterialAssembies = new List<TFAssembly>();
            //List<TFMaterial> materials = new List<TFMaterial>();
            //materials.Add(new TFMaterial() { MaterialName = "Material" });
            //MaterialAssembies.Add(new TFAssembly() { XXX_CO2_M_NAME = "Assembly1", Materials = materials });
            //MaterialAssembies.Add(new TFAssembly() { XXX_CO2_M_NAME = "Assembly2", Materials = materials });
        }

        public static void Initialize()
        {
            PopulateAssemblies();
        }
    }
}
