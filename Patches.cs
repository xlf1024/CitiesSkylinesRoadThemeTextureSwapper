using ColossalFramework;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.Reflection;
using System.Threading;
using UnityEngine;

namespace RoadThemeTextureSwapper
{

    [HarmonyPatch]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("CodeQuality", "IDE0051", Justification = "Called by harmony")]
    class Patches
    {
        [HarmonyDebug]
        [HarmonyTranspiler]
        [HarmonyPatch(typeof(NetSegment), "RenderInstance", new Type[] { typeof(RenderManager.CameraInfo), typeof(ushort), typeof(int), typeof(NetInfo), typeof(RenderManager.Instance) }, new ArgumentType[] { ArgumentType.Normal, ArgumentType.Normal, ArgumentType.Normal, ArgumentType.Normal, ArgumentType.Ref})]
        public static IEnumerable<CodeInstruction> RenderInstanceTranspiler(IEnumerable<CodeInstruction> instructions)
        {
            Debug.Log("transpiling RenderInstance");
            foreach(var instruction in instructions)
            {
                yield return instruction;
                if(instruction.opcode == OpCodes.Callvirt)
                {
                    if (instruction.Calls(typeof(MaterialPropertyBlock).GetMethod("Clear"))){
                        yield return CodeInstruction.Call(typeof(Patches), "ApplyTextures");
                    }
                }
            }
        }
        public static void ApplyTextures()
        {
            var netManager = Singleton<NetManager>.instance;
            var materialBlock = netManager.m_materialBlock;
            foreach (Slots slot in Enum.GetValues(typeof(Slots)))
            {
                materialBlock.SetTexture(slot.ToString(), getTexture(Settings.selection[(int)slot]));
            }
            materialBlock.SetVector("_TerrainTextureTiling1", new Vector4(
                GetTextureTiling(Settings.selection[(int)Slots._TerrainPavementDiffuse]),
                GetTextureTiling(Settings.selection[(int)Slots._TerrainRuinedDiffuse]),
                1f,
                GetTextureTiling(Settings.selection[(int)Slots._TerrainCliffDiffuse])
                ));
            materialBlock.SetVector("_TerrainTextureTiling2", new Vector4(
                GetTextureTiling(Settings.selection[(int)Slots._TerrainGrassDiffuse]),
                GetTextureTiling(Settings.selection[(int)Slots._TerrainGravelDiffuse]),
                1f,
                1f
                ));
        }

        public static Texture getTexture(GlobalTextures textureId)
        {
            var terrainProperties = Singleton<TerrainProperties>.instance;
            var netProperties = Singleton<NetProperties>.instance;
            var buildingProperties = Singleton<BuildingProperties>.instance;
            var vehicleProperties = Singleton<VehicleProperties>.instance;
            switch (textureId)
            {
                case GlobalTextures._TerrainGrassDiffuse:
                    return terrainProperties.m_grassDiffuse;
                case GlobalTextures._TerrainRuinedDiffuse:
                    return terrainProperties.m_ruinedDiffuse;
                case GlobalTextures._TerrainPavementDiffuse:
                    return terrainProperties.m_pavementDiffuse;
                case GlobalTextures._TerrainGravelDiffuse:
                    return terrainProperties.m_gravelDiffuse;
                case GlobalTextures._TerrainCliffDiffuse:
                    return terrainProperties.m_cliffDiffuse;
                case GlobalTextures._TerrainOreDiffuse:
                    return terrainProperties.m_oreDiffuse;
                case GlobalTextures._TerrainOilDiffuse:
                    return terrainProperties.m_oilDiffuse;
                case GlobalTextures._TerrainSandDiffuse:
                    return terrainProperties.m_sandDiffuse;
                case GlobalTextures._RoadUpwardDiffuse:
                    return netProperties.m_upwardDiffuse;
                case GlobalTextures._RoadDownwardDiffuse:
                    return netProperties.m_downwardDiffuse;
                case GlobalTextures._BuildingBaseDiffuse:
                    return buildingProperties.m_baseDiffuse;
                case GlobalTextures._BuildingFloorDiffuse:
                    return buildingProperties.m_floorDiffuse;
                case GlobalTextures._BuildingBurnedDiffuse:
                    return buildingProperties.m_burnedDiffuse;
                case GlobalTextures._BuildingAbandonedDiffuse:
                    return buildingProperties.m_abandonedDiffuse;
                case GlobalTextures._VehicleFloorDiffuse:
                    return vehicleProperties.m_floorDiffuse;
            }
            return null;
        }
        public static float GetTextureTiling(GlobalTextures textureId)
        {
            var terrainProperties = Singleton<TerrainProperties>.instance;
            var netProperties = Singleton<NetProperties>.instance;
            var buildingProperties = Singleton<BuildingProperties>.instance;
            var vehicleProperties = Singleton<VehicleProperties>.instance;
            switch (textureId)
            {
                case GlobalTextures._TerrainGrassDiffuse:
                    return terrainProperties.m_grassTiling;
                case GlobalTextures._TerrainRuinedDiffuse:
                    return terrainProperties.m_ruinedTiling;
                case GlobalTextures._TerrainPavementDiffuse:
                    return terrainProperties.m_pavementTiling;
                case GlobalTextures._TerrainGravelDiffuse:
                    return terrainProperties.m_gravelTiling;
                case GlobalTextures._TerrainCliffDiffuse:
                    return terrainProperties.m_cliffTiling;
                case GlobalTextures._TerrainOreDiffuse:
                    return terrainProperties.m_oreTiling;
                case GlobalTextures._TerrainOilDiffuse:
                    return terrainProperties.m_oilTiling;
                case GlobalTextures._TerrainSandDiffuse:
                    return terrainProperties.m_sandTiling;
                case GlobalTextures._RoadUpwardDiffuse:
                    return 0.0625f;
                case GlobalTextures._RoadDownwardDiffuse:
                    return 0.0625f;
                default:
                    return Settings.defaultTiling;
            }
        }


        // Force all Nets to refetch their Textures.
        public static void RefreshNets()
        {
            Debug.Log("refreshing nets");
            Settings.LogSettings();
            NetManager netManager = Singleton<NetManager>.instance;
            for (ushort i = 0; i < netManager.m_nodes.m_buffer.Length; i++)
            {
                netManager.UpdateNodeRenderer(i, false);
            }
            for (ushort i = 0; i < netManager.m_segments.m_buffer.Length; i++)
            {
                netManager.UpdateSegmentRenderer(i, false);
            }
        }
    }
}