using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Visual;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Xml.Linq;

namespace TwentyFiftyMaterialsRevit.RevitUI
{
    public partial class ConnectorBindingsRevit
    {
        public override void AddMaterial()
        {
            Queue.Add(new Action(() =>
            {
                using (Transaction t = new Transaction(CurrentDoc.Document, "Create New Material"))
                {
                    t.Start();
                    AddMaterial_Unwrapped();
                    t.Commit();
                }
            }));
            Executor.Raise();
        }

        private void AddMaterial_Unwrapped()
        {
            //Define variables
            string matName = "Dummy Material";
            string matClass = "Masonry";
            string assetName = "AppearanceAsset_02";
            string surforePat = "Horizontal  50mm";
            string surbackPat = "<Solid fill>";
            string cutforePat = "Diagonal crosshatch";
            string cutbackPat = "<Solid fill>";
            Color uniColor = new Color(255, 230, 230);
            Color patColor = new Color(0, 0, 0);
            int transPar = 0;

            // Query for an existing material with the requested name
            var solMat = default(Material);
            //Set the file path to the texture
            string texturePath = "";

            if (Material.IsNameUnique(CurrentDoc.Document, matName))
            {
                ElementId newMat = Material.Create(CurrentDoc.Document, matName);
                //Harvest the newly created material from Revit
                solMat = CurrentDoc.Document.GetElement(newMat) as Material;
            }
            else
            {
                using (var collector = new FilteredElementCollector(CurrentDoc.Document))
                {
                    solMat = collector.OfClass(typeof(Material)).
                            WhereParameterEqualsTo(BuiltInParameter.MATERIAL_NAME, matName).
                            FirstElement() as Material;
                }
            }

            //Set the material class
            solMat.MaterialClass = matClass;

            //Set ther color
            solMat.Color = uniColor;

            //Set the material transparency
            solMat.Transparency = transPar;

            //Set the SurfaceForegoundPattern and Color
            solMat.SurfaceForegroundPatternId = FillPatternElement.GetFillPatternElementByName(CurrentDoc.Document, FillPatternTarget.Model, surforePat).Id;
            solMat.SurfaceForegroundPatternColor = patColor;

            //Set the SurfaceBackgroundPattern and Color
            solMat.SurfaceBackgroundPatternId = FillPatternElement.GetFillPatternElementByName(CurrentDoc.Document, FillPatternTarget.Drafting, surbackPat).Id;
            solMat.SurfaceBackgroundPatternColor = uniColor;

            //Set the CutforegroundPattern and Color
            solMat.CutForegroundPatternId = FillPatternElement.GetFillPatternElementByName(CurrentDoc.Document, FillPatternTarget.Drafting, cutforePat).Id;
            solMat.CutForegroundPatternColor = patColor;

            //Set the CutBackgroundPattern and Color
            solMat.CutBackgroundPatternId = FillPatternElement.GetFillPatternElementByName(CurrentDoc.Document, FillPatternTarget.Drafting, cutbackPat).Id;
            solMat.CutBackgroundPatternColor = uniColor;

            //Get your starting ApperanceAssetElement and duplicate it
            AppearanceAssetElement temlAsset = AppearanceAssetElement.GetAppearanceAssetElementByName(CurrentDoc.Document, "Smooth Precast Structural");
            var exsisting = AppearanceAssetElement.GetAppearanceAssetElementByName(CurrentDoc.Document, assetName);

            var newAsset = default(AppearanceAssetElement);
            if (exsisting == null)
            {
                newAsset = temlAsset.Duplicate(assetName);
                solMat.AppearanceAssetId = newAsset.Id;
            }
            else
            {
                newAsset = exsisting;
                solMat.AppearanceAssetId = exsisting.Id;
            }


            //Change the Image In The AppearanceAsset
            using (AppearanceAssetEditScope editEd = new AppearanceAssetEditScope(newAsset.Document))
            {
                Asset editableAsset = editEd.Start(newAsset.Id);
                AssetProperty texTure = editableAsset.FindByName("generic_diffuse");
                Asset connectedAsset = texTure.GetSingleConnectedAsset() as Asset;

                if (connectedAsset.Name == "UnifiedBitmapSchema")
                {
                    AssetPropertyString path = connectedAsset.FindByName(UnifiedBitmap.UnifiedbitmapBitmap) as AssetPropertyString;

                    if (path.IsValidValue(texturePath))
                        path.Value = texturePath;
                }

                editEd.Commit(true);
            }
        }

    }

}
