using ColossalFramework;
using ColossalFramework.UI;
using ICities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace RoadThemeTextureSwapper
{
    class Settings
    {
        public static string FileName => nameof(RoadThemeTextureSwapperMod);

        static Settings()
        {
            if (GameSettings.FindSettingsFileByName(FileName) == null)
            {
                GameSettings.AddSettingsFile(new SettingsFile[] { new SettingsFile() { fileName = FileName } });
            }
        }

        private static UIDropDown[] dropDowns = new UIDropDown[Enum.GetValues(typeof(Slots)).Length];
        private static UITextField defaultTilingInput;
        public static GlobalTextures[] selection = new GlobalTextures[] {
            GlobalTextures._TerrainGrassDiffuse,
            GlobalTextures._TerrainRuinedDiffuse,
            GlobalTextures._TerrainPavementDiffuse,
            GlobalTextures._TerrainGravelDiffuse,
            GlobalTextures._TerrainCliffDiffuse,
            GlobalTextures._RoadUpwardDiffuse,
            GlobalTextures._RoadDownwardDiffuse
        };
        public static void OnSettingsUI(UIHelperBase helper)
        {
            Debug.Log("Make settings was called");
            for (int i = 0; i < Enum.GetValues(typeof(Slots)).Length; i++)
            {
                dropDowns[i] = helper.AddDropdown(((Slots)i).ToString(), Enum.GetNames(typeof(GlobalTextures)), (int)selection[i], (_) => OnSettingsChanged()) as UIDropDown;
            }
            defaultTilingInput = helper.AddTextfield("default tiling value", defaultTiling.ToString(), (_) => { }, (_) => { OnSettingsChanged(); }) as UITextField;
            helper.AddCheckbox("temporarily disable the mod (for quick comparison)", TempDisable, (isChecked) => { TempDisable = isChecked; Patches.RefreshNets(); });
        }

        public static void OnSettingsChanged()
        {
            bool changed = false;
            for (int i = 0; i < dropDowns.Length; i++)
            {
                if (dropDowns[i].selectedIndex != (int)selection[i])
                {
                    changed = true;
                    selection[i] = (GlobalTextures)dropDowns[i].selectedIndex;
                }
            }
            float newDefaultTiling = Util.StringToFloat(defaultTilingInput.text, defaultTiling);
            if (newDefaultTiling != defaultTiling)
            {
                changed = true;
                defaultTilingInput.text = newDefaultTiling.ToString();
                defaultTiling = newDefaultTiling;
            }
            if (changed)
            {
                Patches.RefreshNets();
            }
        }
        public static UITextField createTextField(UIHelperBase helper, string defaultContent, OnTextChanged onTextChanged, OnTextSubmitted onTextSubmitted)
        {
            var textfield = helper.AddTextfield("..", defaultContent, onTextChanged, onTextSubmitted) as UITextField;
            var parent = textfield.parent;
            parent.RemoveUIComponent(textfield);
            var label = parent.Find<UILabel>("Label");
            parent.RemoveUIComponent(label);
            UnityEngine.Object.Destroy(label);
            parent.parent.RemoveUIComponent(parent);
            return textfield;
        }
        public static UISlider createSlider(UIHelperBase helper, float min, float max, float step, float defaultValue, OnValueChanged onValueChanged)
        {
            var slider = helper.AddSlider("..", min, max, step, defaultValue, onValueChanged) as UISlider;
            var parent = slider.parent;
            parent.RemoveUIComponent(slider);
            var label = parent.Find<UILabel>("Label");
            parent.RemoveUIComponent(label);
            UnityEngine.Object.Destroy(label);
            parent.parent.RemoveUIComponent(parent);
            return slider;

        }
        public static bool TempDisable = false;
        public static float defaultTiling = 1f;

        public static void LogSettings()
        {
            Debug.Log(
                "ROTTS map: \n" +
                selection.Select((globalTexture, index) => { return ((Slots)index).ToString() + ": " + globalTexture.ToString(); }).Aggregate((a, b) => { return a + "\n" + b; })
                );
        }
    }
    public enum GlobalTextures
    {
        // from TerrainProperties
        _TerrainGrassDiffuse,
        _TerrainRuinedDiffuse,
        _TerrainPavementDiffuse,
        _TerrainGravelDiffuse,
        _TerrainCliffDiffuse,
        _TerrainOreDiffuse,
        _TerrainOilDiffuse,
        _TerrainSandDiffuse,
        // from NetProperties
        _RoadUpwardDiffuse,
        _RoadDownwardDiffuse,
        // from BuildingProperties
        _BuildingBaseDiffuse,
        _BuildingFloorDiffuse,
        _BuildingBurnedDiffuse,
        _BuildingAbandonedDiffuse,
        // from VehicleProperties
        _VehicleFloorDiffuse
    }
    public enum Slots
    {
        _TerrainGrassDiffuse,
        _TerrainRuinedDiffuse,
        _TerrainPavementDiffuse,
        _TerrainGravelDiffuse,
        _TerrainCliffDiffuse,
        _RoadUpwardDiffuse,
        _RoadDownwardDiffuse


    }
}
