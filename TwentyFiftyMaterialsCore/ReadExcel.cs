using ExcelDataReader;
using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using System.Text;
using TwentyFiftyMaterialsCore.Models;

namespace TwentyFiftyMaterialsCore
{
    internal class ReadExcel
    {
        public static List<TFAssembly> ReadExcelTable()
        {
            DataTable table = null;

            string filePath = "TwentyFiftyMaterialsCore.Database.assembly buildup.xlsx";

            using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(filePath))
            {
                using (var reader = ExcelReaderFactory.CreateReader(stream))
                {
                    do
                    {
                        while (reader.Read())
                        { }
                    } while (reader.NextResult());
                    var result = reader.AsDataSet();
                    table = result.Tables[0];
                }
            }

            int start_row = 1;
            int end_row = 45;
            string assembly_code = "W1";

            
            List<TFAssembly> assemblies = new List<TFAssembly>();
            TFAssembly currentAssembly = new TFAssembly()
            {
                AssemblyCode = assembly_code,
                Materials = new List<TFMaterial>()
            };

            List<TFMaterial> currentMaterials = new List<TFMaterial>();
            

            for (int r = start_row; r < end_row; r++)
            {
                string current_assembly_code = table.Rows[r].ItemArray[0].ToString();

                if (current_assembly_code != assembly_code)
                {
                    currentAssembly.XXX_CO2_M_NAME = table.Rows[r - 1].ItemArray[1].ToString();
                    currentAssembly.Materials = currentMaterials;
                    assemblies.Add(currentAssembly);


                    currentMaterials = new List<TFMaterial>();
                    assembly_code = current_assembly_code;

                    currentAssembly = new TFAssembly()
                    {
                        AssemblyCode = assembly_code,
                        Materials = new List<TFMaterial>()
                    };
                }

                TFMaterial currentMaterial = new TFMaterial()
                {
                    MaterialName = table.Rows[r].ItemArray[3].ToString(),
                    MaterialURL = table.Rows[r].ItemArray[4].ToString(),
                    MaterialThickness= ValueConverter.StringToDouble(table.Rows[r].ItemArray[7].ToString()),
                    MaterialQuantity = ValueConverter.StringToDouble(table.Rows[r].ItemArray[10].ToString())
                };

                currentMaterials.Add(currentMaterial);
            }

            currentAssembly.XXX_CO2_M_NAME = table.Rows[end_row - 1].ItemArray[1].ToString();
            currentAssembly.Materials = currentMaterials;
            assemblies.Add(currentAssembly);
            
            return assemblies;
        }
    }
}
