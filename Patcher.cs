using System.Reflection;
using HarmonyLib;

namespace RoadThemeTextureSwapper {
    public static class Patcher {
        private const string HarmonyId = "xlf1024.RoadThemeTextureSwapper";

        private static bool patched = false;

        public static void PatchAll() {
            if (patched) return;

            UnityEngine.Debug.Log("RoadThemeTextureSwapper: Patching...");

            patched = true;

            // Apply your patches here!
            Harmony.DEBUG = true;
            var harmony = new Harmony("xlf1024.RoadThemeTextureSwapper");
            harmony.PatchAll(Assembly.GetExecutingAssembly());
        }

        public static void UnpatchAll() {
            if (!patched) return;

            var harmony = new Harmony(HarmonyId);
            harmony.UnpatchAll(HarmonyId);

            patched = false;

            UnityEngine.Debug.Log("RoadThemeTextureSwapper: Reverted...");
        }
    }
	/*
    // Random example patch
    [HarmonyPatch(typeof(SimulationManager), "CreateRelay")]
    public static class SimulationManagerCreateRelayPatch {
        public static void Prefix() {
            UnityEngine.Debug.Log("CreateRelay Prefix");
        }
    }

    // Random example patch
    [HarmonyPatch(typeof(LoadingManager), "MetaDataLoaded")]
    public static class LoadingManagerMetaDataLoadedPatch {
        public static void Prefix() {
            UnityEngine.Debug.Log("MetaDataLoaded Prefix");
        }
    }*/
}
