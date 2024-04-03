using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;
using UnityEngine;
using VoidMOLE.Patches;

namespace VoidMOLE
{
    [BepInPlugin(MyGUID, PluginName, VersionString)]
    public class VoidMOLEPlugin : BaseUnityPlugin
    {
        private const string MyGUID = "com.equinox.VoidMOLE";
        private const string PluginName = "VoidMOLE";
        private const string VersionString = "2.0.0";

        private static readonly Harmony Harmony = new Harmony(MyGUID);
        public static ManualLogSource Log = new ManualLogSource(PluginName);

        // Config Entries

        private const string voidLimestoneKey = "Void Limestone";
        private const string voidPlantmatterKey = "Void Plantmatter";
        public static ConfigEntry<bool> voidLimestone;
        public static ConfigEntry<bool> voidPlantmatter;

        // Functions

        private void Awake() {
            voidLimestone = Config.Bind("General", voidLimestoneKey, true, new ConfigDescription("Whether to void Limestone when using the M.O.L.E or Mining Charges"));
            voidPlantmatter = Config.Bind("General", voidPlantmatterKey, true, new ConfigDescription("Whether to void Plantmatter when using the M.O.L.E or Mining Charges"));

            Logger.LogInfo($"PluginName: {PluginName}, VersionString: {VersionString} is loading...");
            Harmony.PatchAll();

            Harmony.CreateAndPatchAll(typeof(MOLEActionPatch));

            Logger.LogInfo($"PluginName: {PluginName}, VersionString: {VersionString} is loaded.");
            Log = Logger;
        }
    }
}
