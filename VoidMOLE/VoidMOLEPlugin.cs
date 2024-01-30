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
        private const string VersionString = "1.0.0";

        private static readonly Harmony Harmony = new Harmony(MyGUID);
        public static ManualLogSource Log = new ManualLogSource(PluginName);

        private void Awake() {
            Logger.LogInfo($"PluginName: {PluginName}, VersionString: {VersionString} is loading...");
            Harmony.PatchAll();

            Harmony.CreateAndPatchAll(typeof(MOLEActionPatch));

            Logger.LogInfo($"PluginName: {PluginName}, VersionString: {VersionString} is loaded.");
            Log = Logger;
        }
    }
}
