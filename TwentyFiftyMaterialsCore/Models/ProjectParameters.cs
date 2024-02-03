﻿using System;
using System.Collections.Generic;
using System.Text;

namespace TwentyFiftyMaterialsCore.Models
{
    public class ProjectParameters
    {
        public static List<TFParameter<string>> StringParameters { get; set; }
        public static List<TFParameter<int>> IntegerParameters { get; set; }
        public static List<TFParameter<double>> DoubleParameters { get; set; }

        public static void PopulateParameters()
        {
            StringParameters = new List<TFParameter<string>>
            {
                new TFParameter<string> { Name = "Custom_Param_1", Visible=true }
            };
            IntegerParameters = new List<TFParameter<int>>  
            {          
                new TFParameter<int> { Name = "Custom_Param_2", Visible=true }
            };
            DoubleParameters = new List<TFParameter<double>>          
            {
                new TFParameter<double> { Name = "Custom_Param_3", Visible=true }
            };
        }
    }
}
