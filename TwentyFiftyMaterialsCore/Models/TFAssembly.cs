using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace TwentyFiftyMaterialsCore.Models
{
    public class TFAssembly : ICloneable
    {
        public string AssemblyCode { get; set; }
        public List<TFMaterial> Materials { get; set; }


        public double XXX_CO2_M_THICKNESS => CalculateThickness();
        public string XXX_CO2_M_NAME { get; set; }
        public string XXX_CO2_M_UNITS => "m2";
        public double XXX_CO2_M_EMBBODIEDCARBON => CalculateEmbodiedCarbon();

        public double TotalCO2 => CalculateTotalCO2();

        public string Element_Guid { get; set; }
        public double Element_Area { get; set; }

        public double CalculateTotalCO2()
        {
            return Element_Area * XXX_CO2_M_EMBBODIEDCARBON;
        }

        public double CalculateEmbodiedCarbon()
        {
            double embodiedCarbon = 0;
            foreach (TFMaterial material in Materials)
            {
                embodiedCarbon += material.CO2perM2;
            }
            return embodiedCarbon;
        }

        public double CalculateThickness()
        {
            double thickness = 0;
            foreach (TFMaterial material in Materials)
            {
                thickness += material.MaterialThickness;
            }
            return thickness;
        }


        #region Clonable Properties
        public virtual TFAssembly DeepClone()
        {
            var json = JsonConvert.SerializeObject(this);
            TFAssembly other = JsonConvert.DeserializeObject<TFAssembly>(json);
            return other;
        }

        public virtual object Clone()
        {
            return this.MemberwiseClone();
        }
        #endregion
    }
}
