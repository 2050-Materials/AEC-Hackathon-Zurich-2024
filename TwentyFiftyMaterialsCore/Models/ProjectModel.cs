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

        }


    }
}
