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

        public static void PopulateAssemblies()
        {
            // Dummy example for populating assemblies
            MaterialAssembies = ReadExcel.ReadExcelTable();
                
            //    new List<TFAssembly>();
            //List<TFMaterial> materials = new List<TFMaterial>();
            //materials.Add(new TFMaterial() { Name = "Material" });
            //MaterialAssembies.Add(new TFAssembly() { Name="Assembly1", Materials = materials });
            //MaterialAssembies.Add(new TFAssembly() { Name = "Assembly2", Materials = materials });

        }

        public static void Initialize()
        {
            PopulateAssemblies();
            ProjectParameters.PopulateParameters();
        }
    }
}
