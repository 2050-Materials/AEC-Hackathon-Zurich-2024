using System;
using System.Collections.Generic;
using System.Text;

namespace TwentyFiftyMaterialsCore.Models
{
    public class TFAssembly
    {
        public string Name { get; set; }

        public List<TFMaterial> Materials { get; set; }

        public double Area { get; set; }
        public double Volume { get; set; }
        public double CO2 { get; set; }

        public double TotalCO2 => Calculate();

        public double Calculate()
        {
            return Area * CO2;
        }
    }
}
