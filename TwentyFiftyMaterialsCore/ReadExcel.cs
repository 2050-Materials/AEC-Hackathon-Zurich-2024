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

            string filePath = "";

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

            int start_row = 0;
            int end_row = 10;
            List < TFAssembly > assemblies = new  List < TFAssembly >();
            for (int r = start_row; r <= end_row; r++)
            {
                TFAssembly assembly = new TFAssembly
                {

                    Name = table.Rows[r].ItemArray[0].ToString(),
                    CO2 = ValueConverter.StringToDouble(table.Rows[r].ItemArray[1].ToString()),
                    Materials = ParseMaterials(table.Rows[r].ItemArray[2].ToString()),
                };

                assemblies.Add(assembly);
            }

            return assemblies;
        }

        private static List<TFMaterial> ParseMaterials(string v)
        {
            return new List<TFMaterial>();
        }
    }
}
