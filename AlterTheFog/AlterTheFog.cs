using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;
using System;
using System.IO;

namespace AlterTheFog
{
    [BepInPlugin(PLUGIN_GUID, PLUGIN_NAME, PLUGIN_VERSION)]
    public class Plugin : BaseUnityPlugin
    {
        public const string PLUGIN_GUID = "com.tinysquid.alterthefog";
        public const string PLUGIN_NAME = "alterthefog";
        public const string PLUGIN_VERSION = "1.0.0";

        private static Harmony harmony;

        private static ConfigEntry<float> aggressiveness;

        private static ConfigEntry<float> initialLevel;
        private static ConfigEntry<float> initialGrowth;
        private static ConfigEntry<float> initialColonize;

        private static ConfigEntry<float> maxDensity;

        private static ConfigEntry<float> growthSpeedFactor;
        private static ConfigEntry<float> powerThreatFactor;
        private static ConfigEntry<float> battleThreatFactor;
        private static ConfigEntry<float> battleExpFactor;

        public void Awake()
        {
            LoadConfig();

            harmony = new Harmony(PLUGIN_GUID);

            ApplyPatches();
        }

        public void OnDestroy()
        {
            RevertPatches();
        }

        public void LoadConfig()
        {
            const string SECTION_KEY = "Difficulty";
            
            aggressiveness = Config.Bind(SECTION_KEY, "aggressiveness", 1f, "Original ranges are -1 to 3");

            initialLevel = Config.Bind(SECTION_KEY, "initialLevel", 0f, "Original ranges are 0 to 10");
            initialGrowth = Config.Bind(SECTION_KEY, "initialGrowth", 1f, "Original ranges are 0 to 6");
            initialColonize = Config.Bind(SECTION_KEY, "initialColonize", 1f, "Original ranges are 0 to 6");
                
            maxDensity = Config.Bind(SECTION_KEY, "maxDensity", 1f, "Original ranges are 0 to 4");
            
            growthSpeedFactor= Config.Bind(SECTION_KEY, "growthSpeedFactor", 1f, "Original ranges are 0 to 4");
            powerThreatFactor = Config.Bind(SECTION_KEY, "powerThreatFactor", 1f, "Original ranges are 0 to 8");
            battleThreatFactor = Config.Bind(SECTION_KEY, "battleThreatFactor", 1f, "Original ranges are 0 to 8");
            battleExpFactor = Config.Bind(SECTION_KEY, "battleExpFactor", 1f, "Original ranges are 0 to 8");

            Logger.LogInfo("Loaded configuration values.");
        }

        public void ApplyPatches()
        {
            try
            {
                harmony.PatchAll(typeof(PatchDarkFogDefault));
                harmony.PatchAll(typeof(PatchDarkFogExport));

                Logger.LogInfo("Patches applied!");
            }
            catch (Exception e)
            {
                Logger.LogError("Patch failed with exception: " + e.ToString());
            }
        }

        public void RevertPatches()
        {
            harmony.UnpatchSelf();
        }

        [HarmonyPatch(typeof(CombatSettings), nameof(CombatSettings.SetDefault))]
        // For new game starts it will use our configured plugin values
        class PatchDarkFogDefault
        {
            public static bool Prefix(ref CombatSettings __instance)
            {
                __instance.aggressiveness = aggressiveness.Value;
                __instance.initialLevel = initialLevel.Value;
                __instance.initialGrowth = initialGrowth.Value;
                __instance.initialColonize = initialColonize.Value;
                __instance.maxDensity = maxDensity.Value;
                __instance.growthSpeedFactor = growthSpeedFactor.Value;
                __instance.powerThreatFactor = powerThreatFactor.Value;
                __instance.battleThreatFactor = battleThreatFactor.Value;
                __instance.battleExpFactor = battleExpFactor.Value;

                return false;
            }
        }

        // For existing saves we have to override the export method so the next save write will reflect our plugin values on reload
        [HarmonyPatch(typeof(CombatSettings), nameof(CombatSettings.Export))]
        class PatchDarkFogExport
        {
            public static bool Prefix(BinaryWriter w)
            {
                w.Write(0);
                w.Write(aggressiveness.Value);

                w.Write(initialLevel.Value);
                w.Write(initialGrowth.Value);
                w.Write(initialColonize.Value);

                w.Write(maxDensity.Value);

                w.Write(growthSpeedFactor.Value);
                w.Write(powerThreatFactor.Value);
                w.Write(battleThreatFactor.Value);
                w.Write(battleExpFactor.Value);

                return false;
            }
        }
    }
}
