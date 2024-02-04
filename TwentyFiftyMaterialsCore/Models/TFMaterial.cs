using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace TwentyFiftyMaterialsCore.Models
{
    public class TFMaterial
    {
        public string MaterialName { get; set; }
        public string MaterialURL { get; set; }
        public double MaterialQuantity { get; set; }
        public double MaterialThickness { get; set; }
        private double CO2 => 1;
        public double CO2perM2 => CalculateCO2perM2();

        internal double CalculateCO2perM2()
        {
            return CO2 * MaterialQuantity;
        }
    }
}
